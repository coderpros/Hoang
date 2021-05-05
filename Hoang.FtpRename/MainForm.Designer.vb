<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.BrowseButton = New System.Windows.Forms.Button()
        Me.UploadButton = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.txtFilePath = New System.Windows.Forms.TextBox()
        Me.SuspendLayout
        '
        'BrowseButton
        '
        Me.BrowseButton.Location = New System.Drawing.Point(12, 12)
        Me.BrowseButton.Name = "BrowseButton"
        Me.BrowseButton.Size = New System.Drawing.Size(75, 23)
        Me.BrowseButton.TabIndex = 0
        Me.BrowseButton.Text = "&Browse"
        Me.BrowseButton.UseVisualStyleBackColor = true
        '
        'UploadButton
        '
        Me.UploadButton.Location = New System.Drawing.Point(12, 42)
        Me.UploadButton.Name = "UploadButton"
        Me.UploadButton.Size = New System.Drawing.Size(75, 23)
        Me.UploadButton.TabIndex = 1
        Me.UploadButton.Text = "&Upload"
        Me.UploadButton.UseVisualStyleBackColor = true
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'txtFilePath
        '
        Me.txtFilePath.Location = New System.Drawing.Point(94, 13)
        Me.txtFilePath.Name = "txtFilePath"
        Me.txtFilePath.Size = New System.Drawing.Size(100, 20)
        Me.txtFilePath.TabIndex = 2
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(205, 128)
        Me.Controls.Add(Me.txtFilePath)
        Me.Controls.Add(Me.UploadButton)
        Me.Controls.Add(Me.BrowseButton)
        Me.MaximizeBox = false
        Me.Name = "MainForm"
        Me.Text = "FTP Rename"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

    Friend WithEvents BrowseButton As Button
    Friend WithEvents UploadButton As Button
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents txtFilePath As TextBox
End Class
