//Duynt
function drawBreadCrumbManual(opt_breadCrumbs) {
    $("#ribbon .breadcrumb").html('<li>Home</li>');
    var lstBreadCrumb = opt_breadCrumbs.split("/");
    var total = lstBreadCrumb.length;
    for (var i = 0; i < total; i++) {
        $("#ribbon .breadcrumb").append('<li>' + lstBreadCrumb[i] + '</li>');
    }
}
// NAMDV
function elementInViewport2(el) {
    var top = el.offsetTop;
    var left = el.offsetLeft;
    var width = el.offsetWidth;
    var height = el.offsetHeight;

    while (el.offsetParent) {
        el = el.offsetParent;
        top += el.offsetTop;
        left += el.offsetLeft;
    }

    return (
        top < (window.pageYOffset + window.innerHeight) &&
        left < (window.pageXOffset + window.innerWidth) &&
        (top + height) > window.pageYOffset &&
        (left + width) > window.pageXOffset
    );
}



function elementInViewport(el) {
    var top = el.offsetTop;
    var left = el.offsetLeft;
    var width = el.offsetWidth;
    var height = el.offsetHeight;

    while (el.offsetParent) {
        el = el.offsetParent;
        top += el.offsetTop;
        left += el.offsetLeft;
    }

    return (
        top >= window.pageYOffset &&
        left >= window.pageXOffset &&
        (top + height) <= (window.pageYOffset + window.innerHeight) &&
        (left + width) <= (window.pageXOffset + window.innerWidth)
    );
}


(function ($) {
    $.fn.fixMe = function () {
        return this.each(function () {
            var $this = $(this),
                $t_fixed;
            function init() {
                $this.wrap('<div class="container" />');
                $t_fixed = $this.clone();
                $t_fixed.find("tbody").remove().end().addClass("fixed").insertBefore($this);
                resizeFixed();
            }
            function resizeFixed() {

            }
            function scrollFixed() {
                var offset = $(this).scrollTop(),
                    tableOffsetTop = $this.offset().top,
                    tableOffsetBottom = tableOffsetTop + $this.height() - $this.find("thead").height();
                if (offset < tableOffsetTop || offset > tableOffsetBottom)
                    $t_fixed.hide();
                else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && $t_fixed.is(":hidden"))
                    $t_fixed.show();
            }
            $(window).resize(resizeFixed);
            $(window).scroll(scrollFixed);
            init();
        });
    };
})(jQuery);


var CommonJS = {
    HEADER_HIGHT: 271,

    baseAlert: function (msg) {
        if (msg.Message != null) {
            alert(msg.Message);
            return msg;
        }
        json = {};
        try {
            json = $.parseJSON(msg);
        } catch (e) {
            alert(msg);
            return;
        }

        alert(json.Message);
        return json;
    },

    alert: function (msg) {
        $("#_GlobalMessage").attr("class", "");
        $("#_GlobalMessage").addClass("msg-type-SUCCESS");
        if (msg.Message != null) {
            $("#_GlobalMessage").html(msg.Message);
            $("#_GlobalMessage").fadeIn();
            setTimeout('$("#_GlobalMessage").fadeOut();', 6000);
            return msg;
        }
        json = {};
        try {
            json = $.parseJSON(msg);
        } catch (e) {
            $("#_GlobalMessage").html(msg);
            $("#_GlobalMessage").fadeIn();
            setTimeout('$("#_GlobalMessage").fadeOut();', 6000);
            return;
        }
        if (json == null) {
            $("#_GlobalMessage").addClass("msg-type-ERROR");
            $("#_GlobalMessage").html("null");
        } else {
            $("#_GlobalMessage").addClass("msg-type-" + json.Type);
            $("#_GlobalMessage").html(json.Message);
        }
        $("#_GlobalMessage").fadeIn();
        setTimeout('$("#_GlobalMessage").fadeOut();', 6000);
        return json;
    },

    back: function () {
        history.back(1);
    },

    checkDate: function checkDate(date) {
        var minYear = 1902;
        var errorMsg = "";
        // regular expression to match required date format 
        re = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/;

        if (date != '') {
            if (regs = date.match(re)) {
                if (regs[1] < 1 || regs[1] > 31) {
                    errorMsg = "Invalid value for day: " + regs[1];
                    return false;
                }
                else if (regs[2] < 1 || regs[2] > 12) {
                    errorMsg = "Invalid value for month: " + regs[2];
                    return false;
                }
                else if (regs[3] < minYear) {
                    errorMsg = "Invalid value for year: " + regs[3] + " - must be between " + minYear;
                    return false;
                }
            }
            else {
                errorMsg = "Invalid date format: " + date;
                return false;
            }
        }
        else {
            errorMsg = "Empty date not allowed!";
            return false;
        }

        return true;
    },
    isNumber: function (n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    },

    makeSizeMatchParent: function (elementID, parentID) {
        var pheight = $("#" + parentID).height();
        var pwidth = $("#" + parentID).width();
        $("#" + elementID).css({
            width: pwidth,
            height: pheight,
        });
    },

    FullScreen: function (elementID, isFull) {
        if (isFull) {
            $("#container").css({
                width: '1px',
                height: '1px',
                overflow: 'hidden'
            });

            $("#" + elementID).css({
                position: 'absolute',
                width: $(window).width() - 3,
                height: $(window).height() - 5,
                top: 0,
                left: 0,
                'z-index': 100,
                overflow: 'hidden',
                background: '#fff'
            });

            $(".form-enter-data").css({
                width: $(window).width() - 290,
                height: $(window).height() - 100,
                'max-height': $(window).height() - 100,
                overflow: 'auto',
            });
            $("#DivGroupTree .panelScroll").width($("#DivGroupTree").width());
        } else {
            $("#container").css({
                width: '',
                height: '',
                overflow: ''
            });
            $("#" + elementID).css({
                position: '',
                width: '',
                height: '',
                top: '',
                left: '',
                'z-index': '',
                overflow: '',
                background: ''
            });

            $(".form-enter-data").css({
                width: '',
                height: '',
                overflow: '',
                'max-height': $(window).height() - CommonJS.HEADER_HIGHT,
                background: ''
            });
            $("#DivGroupTree .panelScroll").width(194);
        }


    },

    EditableFullScreen: function (elementID, isFull) {
        $(window).resize(function () {
            CommonJS.FullScreen(elementID, isFull);
        });
        CommonJS.FullScreen(elementID, isFull);
        if (isFull) {
            $("#btnFullScreen").hide();
            $("#btnUnFull").show();
            $("#TemplateHide").show();
            $("#isFullScreen").val(1);
            $(".PanelContentScroll").css({
                'max-height': $(window).height() - 90,
            });
        } else {
            $("#btnFullScreen").show();
            $("#btnUnFull").hide();
            $("#isFullScreen").val(0);
            $(".PanelContentScroll").css({
                'max-height': $(window).height() - CommonJS.HEADER_HIGHT,
            });
        }
    },

    PrepareEditableFullScreen: function () {
        if ($("#isFullScreen").val() * 1 == 1) {
            $("#btnFullScreen").click();
        } else {
            $("#btnUnFull").click();
        }
    },
    ReloadPage: function () {
        window.location.href = window.location.href;
    },
    ClearAllCache: function () {
        sessionStorage.clear();
    },
    TreeClickFirstLeave: function () {
        var cnode = $($("div.hitarea")[0])
        var _count = 1;
        while (cnode.length > 0) {
            if (!cnode.hasClass("lastCollapsable-hitarea")) {
                cnode.click();
            }
            cnode = $(cnode.parent().find("ul li div.hitarea")[0])
            _count++;
            if (_count > 20) break;
        }

        $($("li span[id^=Template]")[0]).find("a.treeitem").click();
    },
    showNotifySuccess: function (message) {
        notif({
            type: 'success',
            position: 'bottom',
            msg: message,
            autohide: false,
            multiline: true,
            clickable: true
        });
    },
    showNotifyWarning: function (message) {
        notif({
            type: 'warning',
            position: 'bottom',
            msg: message,
            autohide: false,
            multiline: true,
            clickable: true
        });
    },
    showNotifyError: function (message) {
        notif({
            type: 'error',
            position: 'bottom',
            msg: message,
            autohide: false,
            multiline: true,
            clickable: true
        });
    },
    removeVietnameseChars: function (str) {
        str = str.trim().toLowerCase();
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        return str;
    },
    checkDate: function checkDate(date) {
        var minYear = 1902;
        var errorMsg = "";
        // regular expression to match required date format 
        re = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/;
        if (date != '') {
            if (regs = date.match(re)) {
                if (regs[1] < 1 || regs[1] > 31) {
                    errorMsg = "Invalid value for day: " + regs[1];
                    return false;
                }
                else if (regs[2] < 1 || regs[2] > 12) {
                    errorMsg = "Invalid value for month: " + regs[2];
                    return false;
                }
                else if (regs[3] < minYear) {
                    errorMsg = "Invalid value for year: " + regs[3] + " - must be between " + minYear;
                    return false;
                }
            }
            else {
                errorMsg = "Invalid date format: " + date;
                return false;
            }
        }
        else {
            errorMsg = "Empty date not allowed!";
            return false;
        }
        return true;
    },
    dateValidate: function (txtDate) {
        var currVal = txtDate;
        if (currVal == '')
            return true;
        var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/; //Declare Regex
        var dtArray = currVal.match(rxDatePattern); // is format OK?
        if (dtArray == null)
            return false;
        var arrDate = txtDate.split('/');
        var dtDay = arrDate[0];
        var dtMonth = arrDate[1];
        var dtYear = arrDate[2];
        if (dtMonth < 1 || dtMonth > 12)
            return false;
        else if (dtDay < 1 || dtDay > 31)
            return false;
        else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
            return false;
        else if (dtMonth == 2) {
            var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
            if (dtDay > 29 || (dtDay == 29 && !isleap))
                return false;
        }
        return true;
    }
}

//@author: duynn
//@since: 07/08/2018
//@description: format chuỗi dài
function formatLongText(text, size) {
    if (size > 1) {
        if (text.length > size) {
            text = text.substring(0, size - 1);
            text += '...';
        }
    }
    return text;
}