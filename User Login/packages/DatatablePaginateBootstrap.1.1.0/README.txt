This is an extension script to jQuery Datatables.  It allows you to override the default pager for one with a textbox for entering page numbers.  It also implements a bootstrap highlight and glyphicons.  It is customizable and intuitive.

Add the script to the page.  You can add it directly or use a bundle.  It must appear below the script reference to DataTables itself:
<script type="text/javascript" src="~/Scripts/bootstrapPager.min.js"></script>


In your datatable declaration select this plugin:

$('#tbl').DataTable({
    pagingType: "bootstrapPager",
    ...
});


It's as easy as that. If you want to adjust some settings add the pagerSettings object:

$('#tbl').DataTable({
    pagingType: "bootstrapPager",
    pagerSettings: {
       textboxWidth: 70,
       firstIcon: "",
       previousIcon: "glyphicon glyphicon-arrow-left",
       nextIcon: "glyphicon glyphicon-arrow-right",
       lastIcon: "",
       searchOnEnter: true,
       language: "Page ~ of ~ pages"
    },
    ...
});

See more details at: https://github.com/chadkuehn/BootstrapPager