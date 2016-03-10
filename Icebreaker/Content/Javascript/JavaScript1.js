$(ducoment).ready(function () {
    var headerArea = $('#tableHead'); // 拿到navbar的那个dom对象

    var navbar = $('#tableHead');

    var bottom = headerArea.position().top + headerArea.outerHeight(true) //得到header area底部的位置

    $(document).on('DOMMouseScroll mousewheel', function () {
        var scrollY = bottom - window.pageYOffset; //得到header area的offset

        if (scrollY < 10) {

            navbar.attr('fixed')
        } else if (scrollY > 10) {

            navbar.hide();

        }
    });
})