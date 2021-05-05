Imports System.IO
Imports System.Net

Public Class MainForm
    Dim _tmpFileExtension As String
    Dim _tmpFileName As String

    #Region "EVENTS"
    Private Sub BrowseButton_Click(sender As Object, e As EventArgs) Handles BrowseButton.Click
        Try
            With OpenFileDialog1
                .Filter = "Image and Photo files|*.jpg;*.png;*.pdf"
                .FileName = ""
                .ShowDialog()
                txtFilePath.Text = .FileName
                _tmpFileExtension = Path.GetExtension(.FileName)
            End With
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub UploadButton_Click(sender As Object, e As EventArgs) Handles UploadButton.Click
        Try
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            'If cboDocs.SelectedIndex > 0 Then
                If Trim(txtFilePath.Text) <> "" Then
                    _tmpFileName = "blah_cf" & _tmpFileExtension
                    'If cboDocs.SelectedIndex = 1 Then
                    '    tmpFilename = txtShipcode.Text & "_cf" & tmpFileExtension
                    'ElseIf cboDocs.SelectedIndex = 2 Then
                    '    tmpFilename = txtShipcode.Text & "_inv" & tmpFileExtension
                    'ElseIf cboDocs.SelectedIndex = 3 Then
                    '    tmpFilename = txtShipcode.Text & "_img" & tmpFileExtension
                    'ElseIf cboDocs.SelectedIndex = 4 Then
                    '    tmpFilename = txtShipcode.Text & "_kg" & tmpFileExtension
                    'ElseIf cboDocs.SelectedIndex = 5 Then
                    '    tmpFilename = txtShipcode.Text & "_pod" & tmpFileExtension
                    'End If

                    Dim bytes() As Byte = System.IO.File.ReadAllBytes(Trim(txtFilePath.Text))

                    If (bytes.Length / 1048576) < 3 Then
                        FtpUploadFile(Trim(txtFilePath.Text), _tmpFileName)
                        'ListBox1.Items.Add(tmpFilename)
                    Else
                        MsgBox("Maximum size of upload file is 3MB", MsgBoxStyle.Exclamation, "Error")
                        Exit Sub
                    End If
                Else
                    MsgBox("You need to select file to upload.", MsgBoxStyle.Exclamation, "Error")
                    Exit Sub
                End If
            'End If

            Windows.Forms.Cursor.Current = Cursors.Default
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    #End Region

    #Region "PRIVATE HELPERS"
    Private Function FtpGetDirectory() As DataTable
        Dim ftpRequest As System.Net.FtpWebRequest = CType(System.Net.WebRequest.Create($"ftp://{Globals.g_FtpServer}"), System.Net.FtpWebRequest)
        
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails
        ftpRequest.Credentials = New System.Net.NetworkCredential(Globals.g_FtpUserName, Globals.g_FtpPassword)
        ftpRequest.UsePassive = true
        
        Dim response As FtpWebResponse = ftpRequest.GetResponse()
        Dim entries As List(Of String)

        Using reader As New StreamReader(response.GetResponseStream())
            entries = reader.ReadToEnd().Split(new String() { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList()
        End Using

        response.Close()
        
        Dim dtFiles = new DataTable()
        
        dtFiles.Columns.AddRange(
            { 
                New DataColumn("Name", GetType(String)),
                New DataColumn("Size", GetType(Decimal)),
                New DataColumn("Date", GetType(String))
            })

        For Each entry As String In entries
            Dim splits As String() = entry.Split(new String() { " " }, StringSplitOptions.RemoveEmptyEntries)
            
            ' Determine whether entry is for File or Directory.
            Dim isFile As Boolean = splits(0).Substring(0, 1) <> "d"
            Dim isDirectory As Boolean = splits(0).Substring(0,1) = "d"

             If isFile Then
                 dtFiles.Rows.Add()
                 
                 dtFiles.Rows(dtFiles.Rows.Count - 1)("Size") = decimal.Parse(splits(4)) / 1024
                 dtFiles.Rows(dtFiles.Rows.Count - 1)("Date") = string.Join(" ", splits(5), splits(6), splits(7))
                 dtFiles.Rows(dtFiles.Rows.Count - 1)("Name") = splits(8).Trim()
             End If
        Next 

        Return dtFiles
    End Function

    Private Function FtpFileExists(ByVal ftpUri As String) As Boolean
        Dim ftpRequest As System.Net.FtpWebRequest = CType(System.Net.WebRequest.Create($"ftp://{Globals.g_FtpServer}" & ftpUri), System.Net.FtpWebRequest)
        
        ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize
        ftpRequest.Credentials = New System.Net.NetworkCredential(Globals.g_FtpUserName, Globals.g_FtpPassword)

        Try
            ftpRequest.GetResponse()
            Return True
        Catch ex As WebException
            Dim response As FtpWebResponse= ex.Response 
            
            If response.StatusCode = System.Net.FtpStatusCode.ActionNotTakenFileUnavailable Then    
                Return False
            End If
        End Try
        
        Return False
    End Function
    Private Sub FtpUploadFile(ByVal filetoupload As String, ByVal ftpuri As String)
        Try
            ' Check if the file already exists. 
            If Me.FtpFileExists(ftpuri) Then
                ' Get all the files with the same prefix. 
                Dim files As DataTable = FtpGetDirectory()
                Dim fileNameOnly As String = ftpuri.Substring(0, InStrRev(ftpuri, ".") - 1)
                Dim matchCount = 0

                ' Rename the file to the greatest index with the same prefix.
                For Each file as DataRow In files.Rows
                    If file("Name").ToString().Contains(fileNameOnly) Then
                        matchCount = matchCount + 1
                    End If
                Next

                ftpuri = $"{fileNameOnly}_{matchCount}{Me._tmpFileExtension}"
            End If
            
            ' Create a web request that will be used to talk with the server and set the request method to upload a file by ftp.
            Dim ftpRequest As System.Net.FtpWebRequest = CType(System.Net.WebRequest.Create($"ftp://{Globals.g_FtpServer}" & ftpuri), System.Net.FtpWebRequest)

            ftpRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile
            
            ' Confirm the Network credentials based on the user name and password passed in.
            ftpRequest.Credentials = New System.Net.NetworkCredential(Globals.g_FtpUserName, Globals.g_FtpPassword)

            ' Read into a Byte array the contents of the file to be uploaded 
            Dim bytes() As Byte = System.IO.File.ReadAllBytes(filetoupload)

            ' Transfer the byte array contents into the request stream, write and then close when done.
            ftpRequest.ContentLength = bytes.Length

            Using uploadStream As System.IO.Stream = ftpRequest.GetRequestStream()
                uploadStream.Write(bytes, 0, bytes.Length)
                uploadStream.Close()
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Exit Sub
        End Try

        MessageBox.Show($"{_tmpFileName } has been uploaded to server successfully.")
    End Sub

    #End Region
End Class
