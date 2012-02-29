<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Html.Resource("HomePage")%>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Resource("HomePage")%></h2>
    <p>
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam sit amet ante felis, id cursus quam. Nunc et nibh diam. Nunc ut eros odio. Morbi vitae leo euismod massa pharetra pellentesque. Nulla pretium arcu id sem faucibus sed ultricies felis placerat. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vestibulum a est mauris, a consectetur metus. Duis sed leo sed nisl accumsan mollis. Curabitur bibendum neque ac mi gravida tristique. Nunc dui massa, venenatis ac pharetra at, adipiscing vel mi. Fusce sed convallis nisi. Fusce sollicitudin, nulla id bibendum tempor, massa purus mattis quam, auctor auctor ante orci non eros. Fusce lorem quam, placerat id gravida in, tincidunt sed tortor. Etiam mattis, lectus eu malesuada auctor, mauris elit accumsan nisl, ac aliquet erat libero eget ante. Nulla vitae augue eget velit auctor posuere. Ut blandit porttitor sem, eu vulputate odio pellentesque ac.
    </p>
    <p>
Nulla facilisi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nulla convallis gravida diam vel lacinia. Suspendisse in purus massa. Vivamus tempor tellus in lacus pellentesque quis semper nisl dictum. Aenean sodales, dolor malesuada auctor iaculis, eros nisl lobortis dui, quis lacinia ligula eros quis orci. In id ipsum ac risus luctus tempus consequat sit amet magna. Sed ac nisl ac felis cursus pretium. Maecenas turpis purus, iaculis posuere pharetra facilisis, scelerisque ac dolor. Vivamus felis ligula, congue sit amet sagittis ut, accumsan et est. Morbi sed turpis quam, at adipiscing felis. Maecenas turpis odio, vehicula vitae laoreet ac, faucibus nec elit. Pellentesque enim sapien, egestas in luctus mollis, semper et nibh. In hac habitasse platea dictumst. Maecenas iaculis tincidunt arcu, eu pellentesque odio eleifend ut. Nunc dictum ultrices iaculis. Donec diam libero, placerat et ultricies vel, accumsan eu tellus. In hac habitasse platea dictumst. Morbi nulla nunc, ultrices eu semper vel, elementum viverra dui.
    </p>
    <p>
Nunc in orci ac odio interdum fringilla. Proin ac elementum lorem. Suspendisse at orci at ipsum lobortis aliquet sit amet scelerisque sem. Nunc ut risus erat, eu ultrices justo. Quisque eget ante nec lectus egestas facilisis id id turpis. Aliquam molestie nisl tempus leo fringilla ac dictum odio convallis. Mauris tellus justo, vulputate non auctor a, tincidunt id tortor. Mauris ac magna orci, quis luctus est. Nulla eu sapien eu tellus semper consectetur quis in neque. Nam fringilla gravida nunc nec volutpat. Curabitur molestie nisl id metus posuere fermentum. Quisque sed lorem erat. Nam facilisis pretium porta. Phasellus vel leo vel nisi sodales consectetur non nec leo. 
    </p>
</asp:Content>
