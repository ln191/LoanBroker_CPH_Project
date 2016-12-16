<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Client.aspx.cs" Inherits="WebClient.Client" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #form1 {
            height: 279px;
            width: 509px;
            position: relative;
            top: 0px;
            left: 0px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 504px; background-color: #0000CC">
    
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Loan Broker Client</div>
        <p>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SNN&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Amount&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Duration</p>
        <p>
            <asp:TextBox ID="SSN" runat="server"></asp:TextBox>
            <asp:TextBox ID="Amount" runat="server"></asp:TextBox>
            <asp:TextBox ID="Duration" runat="server"></asp:TextBox>
        </p>
        <asp:Button ID="Button1" runat="server" Height="80px" OnClick="Button1_Click" Text="Get A Loan Offer" Width="500px" />
        <br />
        <br />
        <asp:Label ID="Offer" runat="server" Text="Label" Visible="False"></asp:Label>
    </form>
</body>
</html>
