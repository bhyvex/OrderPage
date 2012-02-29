<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
	/*      WIDE LAYOUT START      */
	function adjustStyle(width) {
		width = parseInt(width);
		if (width < 1230) {
			$('body').addClass('narrow_layout');
		} else {
			$('body').removeClass('narrow_layout');
		}
	}
	/*      WIDE LAYOUT END      */
	$(document).ready(function() {
		$(".autocomplete_off").attr("autocomplete", "off");
		
		/*      WIDE LAYOUT START     */
		adjustStyle($(this).width());
		$(window).resize(function() {
			adjustStyle($(this).width());
		});
		/*      WIDE LAYOUT END      */
	});
</script>