Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class Form1

    ' --- SQL Server connection string ---
    Private connectionString As String = "Data Source=MOYANTHA-PC\MSSQLSERVER03;Initial Catalog=CafConnectDB;Integrated Security=True;Encrypt=False"

    ' --- FlowLayoutPanel to hold all vendor cards ---
    Private flowLayoutPanel1 As New FlowLayoutPanel()

    ' --- Constructor for the form ---
    Public Sub New()
        InitializeComponent()

        ' --- Greeting label ---
        Dim lblGreeting As New Label()
        lblGreeting.Text = "Hello!"  ' Later, replace with the logged-in username
        lblGreeting.Font = New Font("Segoe UI", 20, FontStyle.Bold)
        lblGreeting.ForeColor = Color.Navy
        lblGreeting.AutoSize = True
        lblGreeting.Location = New Point(20, 10)
        Me.Controls.Add(lblGreeting)

        ' --- Caption label ---
        Dim lblCaption As New Label()
        lblCaption.Text = "Choose your favorite vendor for the day"
        lblCaption.Font = New Font("Segoe UI", 16, FontStyle.Regular)
        lblCaption.ForeColor = Color.DarkSlateGray
        lblCaption.AutoSize = True
        lblCaption.Location = New Point(20, 55)
        Me.Controls.Add(lblCaption)

        ' --- Initialize FlowLayoutPanel ---
        flowLayoutPanel1.Name = "flowLayoutPanel1"
        flowLayoutPanel1.Dock = DockStyle.Fill  ' Fill the form
        flowLayoutPanel1.FlowDirection = FlowDirection.TopDown  ' Cards stack vertically
        flowLayoutPanel1.WrapContents = False  ' Prevent wrapping to next column
        flowLayoutPanel1.AutoScroll = True  ' Add scroll bar if content overflows
        flowLayoutPanel1.Padding = New Padding(20, 120, 20, 20)  ' Leave space for headings
        flowLayoutPanel1.BackColor = Color.LightGray
        Me.Controls.Add(flowLayoutPanel1)
    End Sub

    ' --- Event: Form Load ---
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Load vendor data when the form opens
        LoadVendors()
    End Sub

    ' --- Load vendors from database ---
    Private Sub LoadVendors()
        ' Clear existing controls in FlowLayoutPanel
        flowLayoutPanel1.Controls.Clear()

        ' SQL query to get VendorID, ShopNumber, ShopDescription
        Dim query As String = "SELECT VendorID, ShopNumber, ShopDescription FROM Vendor"

        ' Connect to database
        Using conn As New SqlConnection(connectionString)
            Dim adapter As New SqlDataAdapter(query, conn)
            Dim vendorTable As New DataTable()
            adapter.Fill(vendorTable)  ' Fill DataTable with query results

            ' Loop through each vendor row
            For Each row As DataRow In vendorTable.Rows
                ' --- Create card panel ---
                Dim card As New Panel()
                card.Width = 450
                card.Height = 150
                card.Margin = New Padding(10)  ' Space between cards
                card.BackColor = Color.WhiteSmoke
                card.Padding = New Padding(0)
                card.Cursor = Cursors.Hand  ' Change cursor to hand over card
                card.Region = New Region(RoundRect(card.ClientRectangle, 15)) ' Rounded corners

                ' --- Header panel for shop name ---
                Dim header As New Panel()
                header.BackColor = Color.Navy
                header.Height = 35
                header.Dock = DockStyle.Top
                header.Region = New Region(RoundRect(New Rectangle(0, 0, card.Width, header.Height), 15, True))
                card.Controls.Add(header)

                ' --- Shop name label inside header ---
                Dim lblName As New Label()
                lblName.Text = row("ShopNumber").ToString()
                lblName.Font = New Font("Segoe UI Semibold", 12, FontStyle.Bold)
                lblName.ForeColor = Color.White
                lblName.AutoSize = True
                lblName.Location = New Point(10, 7)
                header.Controls.Add(lblName)

                ' --- Placeholder for image ---
                Dim pbPlaceholder As New Panel()
                pbPlaceholder.Width = 80
                pbPlaceholder.Height = 80
                pbPlaceholder.Location = New Point(10, 50)
                pbPlaceholder.BackColor = Color.Gray  ' Placeholder color
                pbPlaceholder.BorderStyle = BorderStyle.FixedSingle
                card.Controls.Add(pbPlaceholder)

                ' --- Shop description label ---
                Dim lblDesc As New Label()
                lblDesc.Text = row("ShopDescription").ToString()
                lblDesc.Font = New Font("Segoe UI", 10, FontStyle.Regular)
                lblDesc.ForeColor = Color.DarkGray
                lblDesc.Location = New Point(100, 50)
                lblDesc.MaximumSize = New Size(330, 0)  ' Wrap text within 330px
                lblDesc.AutoSize = True
                card.Controls.Add(lblDesc)

                ' --- "View Menu" button ---
                Dim btnView As New Button()
                btnView.Text = "View Menu"
                btnView.BackColor = Color.Navy
                btnView.ForeColor = Color.White
                btnView.FlatStyle = FlatStyle.Flat
                btnView.FlatAppearance.BorderSize = 0
                btnView.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                btnView.Size = New Size(100, 30)
                btnView.Location = New Point(100, 110)
                AddHandler btnView.Click, Sub(s, e) ViewMenu(CInt(row("VendorID")))
                ' --- Hover effect for button ---
                AddHandler btnView.MouseEnter, Sub(s, e) btnView.BackColor = Color.MidnightBlue
                AddHandler btnView.MouseLeave, Sub(s, e) btnView.BackColor = Color.Navy
                card.Controls.Add(btnView)

                ' --- Add card to FlowLayoutPanel ---
                flowLayoutPanel1.Controls.Add(card)
            Next
        End Using
    End Sub

    ' --- Open vendor menu (placeholder) ---
    Private Sub ViewMenu(vendorId As Integer)
        MessageBox.Show("Opening menu for Vendor ID: " & vendorId)
        ' Later: open another form and pass vendorId
    End Sub

    ' --- Helper: Create rounded rectangle path ---
    ' rect: Rectangle to round
    ' radius: Corner radius
    ' topOnly: Only round top corners (for header)
    Private Function RoundRect(rect As Rectangle, radius As Integer, Optional topOnly As Boolean = False) As GraphicsPath
        Dim path As New GraphicsPath()
        If topOnly Then
            ' Only round top-left and top-right corners
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90)
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90)
            path.AddLine(rect.Right, rect.Bottom, rect.X, rect.Bottom) ' Bottom edge straight
            path.CloseFigure()
        Else
            ' Round all corners
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90)
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90)
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90)
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90)
            path.CloseFigure()
        End If
        Return path
    End Function

End Class

