/*!
 * Start Bootstrap - SB Admin 2 v3.3.7+1 (http://startbootstrap.com/template-overviews/sb-admin-2)
 * Copyright 2013-2016 Start Bootstrap
 * Licensed under MIT (https://github.com/BlackrockDigital/startbootstrap/blob/gh-pages/LICENSE)
 */
$(function() {
    $('#side-menu').metisMenu();
});

//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
// Sets the min-height of #page-wrapper to window size
$(function() {
    $(window).bind("load resize", function() {
        var topOffset = 50;
        var width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse');
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse');
        }

        var height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;
        if (height < 1) height = 1;
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
        }
    });

    var url = window.location;
    // var element = $('ul.nav a').filter(function() {
    //     return this.href == url;
    // }).addClass('active').parent().parent().addClass('in').parent();
    var element = $('ul.nav a').filter(function() {
        return this.href == url;
    }).addClass('active').parent();

    while (true) {
        if (element.is('li')) {
            element = element.parent().addClass('in').parent();
        } else {
            break;
        }
    }
});

function main() {
    (function () {
        'use strict';
        $(document).ready(function () {
            //code here

        })

    }());

}
main();

function goBack() {
    window.history.back();
}

function getExtension(path) {
    var basename = path.split(/[\\/]/).pop(),  // extract file name from full path ...
                                               // (supports `\\` and `/` separators)
        pos = basename.lastIndexOf(".");       // get last position of `.`

    if (basename === "" || pos < 1)            // if file name is empty or ...
        return "";                             //  `.` not found (-1) or comes first (0)

    return basename.slice(pos + 1);            // extract extension ignoring `.`
}

//function formatDate(date) {
//    var hours = date.getHours();
//    var minutes = date.getMinutes();
//    var ampm = hours >= 12 ? 'pm' : 'am';
//    hours = hours % 12;
//    hours = hours ? hours : 12; // the hour '0' should be '12'
//    minutes = minutes < 10 ? '0' + minutes : minutes;
//    var strTime = hours + ':' + minutes + ' ' + ampm;
//    return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear() + "  " + strTime;
//}

function formatDateDD(date) {
    return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear();
}

function toDate(s) {
    if (s !== undefined) {
        var from = s.split(' ');
        var from2 = from[0].split('/');
        var from3 = from[1].split(':');
        return new Date(from2[2], from2[0] - 1, from2[1], from3[0]);
    }
    return false;
}

//function UploadDropzone() {
//    Dropzone.autoDiscover = false;
//    $("#dZUpload1").dropzone({
//        url: "/Account/SaveAnh1",
//        addRemoveLinks: true,
//        maxFiles: 1,
//        maxFilesize: 10,
//        uploadMultiple: true,
//        acceptedFiles: "image/*",
//        dictFallbackMessage: "Trình duyệt của bạn không hỗ trợ kéo thả tệp để tải lên.",
//        dictFallbackText: "Please use the fallback form below to upload your files like in the olden days.",
//        dictFileTooBig: "Tệp có dung lượng quá lớn ({{filesize}}MiB). Dung lượng cho phép: {{maxFilesize}}MiB.",
//        dictInvalidFileType: "Tệp bạn chọn không được phép tải lên.",
//        dictResponseError: "Server responded with {{statusCode}} code.",
//        dictCancelUpload: "Hủy tải lên",
//        dictCancelUploadConfirmation: "Bạn chắc chắn muốn hủy tải lên?",
//        dictRemoveFile: "Loại bỏ tệp tin",
//        dictMaxFilesExceeded: "Bạn không thể tải lên nhiều tệp.",
//        success: function (file, response) {
//            var imgPath = response.Message;
//            console.log(response);
//            file.previewElement.classList.add("dz-success");
//        },

//        error: function (file, response) {
//            file.previewElement.classList.add("dz-error");
//            $(file.previewElement).find('.dz-error-message').text(response);
//        },
//        HiddenFilesPath: 'body'
//    });
//};
//UploadDropzone();

function popitup(url, windowName) {
    newwindow = window.open(url, windowName, 'height=600,width=500,toolbar=0,menubar=0,location=0');
    if (window.focus) { newwindow.focus(); }
    return false;
}

function compareTime(time1, time2) {
    return new Date(time1) > new Date(time2); // true if time1 is later
}

function compareDate(str, end) {
    //var str = document.getElementById("start_date").value;
    //var end = document.getElementById("end_date").value;
    var year = str.split('-')[0];
    var month = str.split('-')[1];
    var date = str.split('-')[2];
    var endYear = end.split('-')[0];
    var endMonth = end.split('-')[1];
    var endDate = end.split('-')[2];
    var startDate = new Date(year, month, date);
    var endDate = new Date(endYear, endMonth, endDate);

    if (startDate > endDate) {        
        return false;
    }
    else { return true; }
}
