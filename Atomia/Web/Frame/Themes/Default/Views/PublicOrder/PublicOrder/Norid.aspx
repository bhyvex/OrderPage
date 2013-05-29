<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage`1[[Atomia.Web.Plugin.PublicOrder.Models.SubmitForm, Atomia.Web.Plugin.PublicOrder]]" %>
<%@ Assembly Name="Atomia.Web.Plugin.DomainSearch" %>
<%@ Assembly Name="Atomia.Web.Plugin.Cart" %>
<%@ Import Namespace="Atomia.Web.Plugin.Cart.Models" %>
<%@ Import Namespace="Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService" %>
<%@ Import Namespace="Atomia.Web.Plugin.PublicOrder.Models"%>
<%@ Import Namespace="System.Collections.Generic"%>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Atomia.Billing.Core.Common.PaymentPlugins" %>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html lang="no"><head profile="http://www.w3.org/2000/08/w3c-synd/#"><meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<meta name="generator" content="Handcoded">
<title>Egenerklæring</title>
<style type="text/css">
<!--
body { background: #ffffff; color: #000000; font-family: Arial, Verdana }
-->
</style>
</head>
<body>
<img src="/Themes/Default/Content/img/layout/no_svart.png" style="clear:both;" alt="Norid .no" title="Norid .no">

<h1>Egenerklæring</h1>

<p>
Denne erklæringen utgjør sammen med det regelverket som gjelder til enhver tid kontrakten mellom UNINETT Norid AS, senere kalt Norid, og abonnenten. Ved signering av erklæringen binder abonnenten seg til vilkårene for å registrere domenenavn under .no.
</p>

<p>
Abonnenten erklærer med dette at registrering og/eller bruk av domenet<br>
- ikke er i strid med gjeldende regelverk, se http://www.norid.no/navnepolitikk.html<br>
- etter abonnentens kunnskap ikke medfører urettmessige inngrep i tredjeparts registre eller uregistrerte rettigheter til navnet<br>
- ikke er i strid med norsk eller internasjonal rett<br>
- ikke uberettiget gir inntrykk av å angå offentlig forvaltning eller myndighetsutøvelse
</p>

<p>
Abonnenten samtykker til<br>
- at klager på abonnentens registrering kan behandles av domeneklagenemnda i henhold til de regler og prosedyrer som følger av regelverket, og at abonnenten vil delta i klageprosessen og være bundet av domeneklagenemndas avgjørelse<br>
- å holde seg oppdatert om regelverksendringer på www.norid.no og følge det regelverket som gjelder til enhver tid<br>
- at Norid kan trekke tilbake et registrert domenenavn når det er åpenbart at tildelingen er i strid med forhold som er nevnt i denne egenerklæringen, eller som på annen måte er i strid med regelverket eller formålet bak regelverket<br>
- at kontaktinformasjon og registreringstidspunkt for domenet gjøres tilgjengelig på Internett, blant annet gjennom Norids whois-database. Tilgang vil også kunne bli gitt via annen internett-teknologi
</p>

<p>
Abonnenten aksepterer at Norid ikke kan holdes ansvarlig for direkte eller indirekte skade som følge av registrarens håndtering av søknader om domenenavn. Abonnenten aksepterer videre at Norid ikke kan holdes ansvarlig overfor abonnenten for noen direkte eller indirekte skade som følge av driftsfeil eller driftsstans hos Norid når det inntrufne skyldes forhold eller omstendigheter som Norid ikke kontrollerer.
</p>

<div>
    Domenenavn: <div style="margin:-18px 0 0 100px"><%= ViewData["domains"].ToString().Replace("|","<br>") %></div><br><br>
    Søkers navn: <%= ViewData["company"].ToString() %><br><br>
    ID-nummer (org.nr eller PID): <%= ViewData["orgid"].ToString() %><br><br>
    Navn på personen som avgir samtykke: <%= ViewData["name"].ToString() %><br><br>
    <span style="float:right">Versjonsnummer: <%= Html.Resource("VersionNumber") %></span>
    Tidsstempel: <%= ViewData["time"].ToString() %>
</div>


</body></html>
</asp:Content>