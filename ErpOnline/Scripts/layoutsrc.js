$('.link').on("click", function () {
    $('#modalLoad').modal("show");
})

$(document).ready(function () {
    $('#page-wrapper').addClass('margin0');
    $('#divside').addClass('sbh');

    var lgurl = $("#logout-url").data("logout");
   
    $('.sidebar-collapse').find('a[href]').removeClass('active-menu');
    var thi = $('.sidebar-collapse').find('a[href="' + location.pathname + '"]').addClass('active-menu');
    thi.parents("ul").addClass("in");

    $('[data-toggle="popover"]').popover({
        placement: 'bottom',
        html: true,
        title: '',
        content: '<a href="#" style="text-decoration: none;">View Profile</a><hr style="margin-top:5px;margin-bottom: 5px;"><a href="' + lgurl + '" class="link" style="text-decoration: none;">Logout</a>'
    });

    $(document).on("click", ".popover .close", function () {
        $(this).parents(".popover").popover('hide');
    });

    $('input').addClass("form-control");

    $(document).on("click", ".navbar-toggle", function () {
        if ($(this).hasClass('collapsed')) {
            $('#page-wrapper').addClass('margin0');
            $('#divside').addClass('sbh');
        } else {
            $('#page-wrapper').removeClass('margin0');
            $('#divside').removeClass('sbh');
        }
    })
})