function FormInvalid(form) {
    var result = true;
    //trim text
    $("#" + form + " input").each(function () {
        if ($(this).attr("type") != "file") {
            var data = $(this).val();
            $(this).val(data.trim());
        }
    });
    //trim textarea
    $("#" + form + " textarea").each(function () {
        var data = $(this).val();
        $(this).val(data.trim());
    });

    //validate require input
    $("#" + form + " .require").each(function (index, value) {
        var data = $(this).val();
        var $parent = $(this).parent();
        if (data == "" || data.length == 0) {
            $parent.find(".is-error").text("Trường dữ liệu không được để trống").addClass("show-error");
            result = false;
        } else {
            $parent.find(".is-error").removeClass("show-error");
        }
    });
    //validate number
    $("#" + form + " .require-number").each(function (index, value) {
        var data = $(this).val();
        var $parent = $(this).parent();
        if (data != "" || data.length > 0) {
            var regex = /^[0-9]{1,10}$/;
            console.log(regex.test(data));
            if (!regex.test(data)) {

                $parent.find(".is-error").text("Bạn phải nhập số").addClass("show-error");
                result = false;
            }
            else {
                $parent.find(".is-error").removeClass("show-error");
            }
        }

    });
    //validate email
    $("#" + form + " .require-email").each(function (index, value) {
        var data = $(this).val();
        if (data != "" && data.length > 0) {
            var $parent = $(this).parent();
            var regex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            if (!regex.test(data)) {
                $parent.find(".is-error").text("Email không hợp lệ").addClass("show-error");
                result = false;
            } else {
                $parent.find(".is-error").removeClass("show-error");
            }
        }
    });

    //validate phone
    $("#" + form + " .require-phone").each(function (index, value) {
        var data = $(this).val();
        if (data != "" && data.length > 0) {
            var $parent = $(this).parent();
            var regex = /^[0-9]{10,15}$/;
            if (!regex.test(data)) {
                $parent.find(".is-error").text("Số điện thoại không đúng").addClass("show-error");
                result = false;
            } else {
                $parent.find(".is-error").removeClass("show-error");
            }
        }
    });
    //validate CMND
    $("#" + form + " .require-cmnd").each(function (value, index) {
        var data = $(this).val();
        if (data != "" && data.length > 0) {
            var $parent = $(this).parent();
            var regex = /^([0-9]+{9} || [0-9]+{12})$/;
            if (!regex.test(data)) {
                $parent.find(".is-error").text("Số CMND không đúng").addClass("show-error");
                result = false;
            } else {
                $parent.find(".is-error").removeClass("show-error");
            }
        }
    });

    //validate Date and Compare Date
    $("#" + form + " .datepicker").each(function (value, index) {
        var data = $(this).val();
        var $dom = $(this);
        if (data != "" && data.length > 0) {
            var $parent = $(this).parent();
            var date_regex = /^\d{1,2}\/\d{1,2}\/\d{4}$/;
            if (!date_regex.test(data)) {
                $parent.find(".is-error").text("Ngày sai định dạng").addClass("show-error");
                result = false;
            }
            var dateParts = data.split("/");
            if (dateParts.length != 3) {
                $parent.find(".is-error").text("Ngày sai định dạng").addClass("show-error");
                result = false;
            }
            var year = dateParts[2];
            var month = dateParts[1];
            var day = dateParts[0];

            if (isNaN(day) || isNaN(month) || isNaN(year)) {
                $parent.find(".is-error").text("Ngày sai định dạng").addClass("show-error");
                result = false;
            }

            var dateResult = new Date(year, month, day);
            if (dateResult == null) {
                $parent.find(".is-error").text("Ngày sai định dạng").addClass("show-error");
                result = false;
            }
            $parent.find(".is-error").removeClass("show-error");
            //check compare min date vs max date
            var attrMaxDate = $dom.attr("data-error-max-date");
            if (typeof (attrMaxDate) != 'undefined' && attrMaxDate != null) {
                var $maxDate = $("#" + form + " [name=" + attrMaxDate + "]");
                if (typeof ($maxDate) != 'undefined' && $maxDate != null) {
                    var dataMaxDate = $maxDate.val();
                    if (dataMaxDate.length > 0) {
                        var dateSplit = dataMaxDate.split("/");
                        if (dateSplit.length == 3) {
                            var dateMax = new Date(dateSplit[2], dateSplit[1], dateSplit[0]);
                            if (dateMax != null) {
                                if (dateResult >= dateMax) {
                                    $parent.find(".is-error").text("Ngày bắt đầu phải nhỏ hơn ngày kết thúc").addClass("show-error");
                                    result = false;
                                } else {
                                    var $parentMaxDate = $maxDate.parent();
                                    $parentMaxDate.find(".is-error").removeClass("show-error");
                                }
                            }
                        }
                    }
                }

            }
            //check compare max date vs min date
            var attrMinDate = $dom.attr("data-error-min-date");
            if (typeof (attrMinDate) != 'undefined' && attrMinDate != null) {
                var $minDate = $("#" + form + " [name=" + attrMinDate + "]");
                if (typeof ($minDate) != 'undefined' && $minDate != null) {
                    var dataMinDate = $($minDate).val();
                    if (dataMinDate.length > 0) {
                        var dateSplit = dataMinDate.split("/");
                        if (dateSplit.length == 3) {
                            var dateMin = new Date(dateSplit[2], dateSplit[1], dateSplit[0]);
                            if (dateMin != null) {
                                if (dateMin >= dateResult) {
                                    $parent.find(".is-error").text("Ngày kết thúc phải lớn hơn ngày bắt đầu").addClass("show-error");
                                    result = false;
                                } else {
                                    var $parentMinDate = $minDate.parent();
                                    $parentMinDate.find(".is-error").removeClass("show-error");
                                }
                            }
                        }
                    }
                }

            }
        }
    });
    //remove existed ajax server
    $(document).on("change", "#" + form + " [data-error-existed]", function () {
        var $current = $(this);
        var data = $(this).val();
        if (data != "" && data.length > 0) {
            var name = $(this).attr("name").toString();
            var url = $current.attr("data-error-existed") + "?" + name + "=" + data;
            $.ajax({
                type: "post",
                url: url,
                cache: false,
                dataType: 'json',
                success: function (response) {
                    if (response.Status) {
                        $current.parent().find(".is-error").text("Dữ liệu đã tồn tại").addClass("show-error");
                        result = false;
                    } else {
                        $current.parent().find(".is-error").removeClass("show-error");
                    }
                }, error: function (xhr) {
                    CommonJS.NotifyError("Đã có lỗi xảy ra");
                }
            });
        }
    });
    return result;
}

function SetupFormError(form) {
    //create tag error
    $("#" + form + " .has-errored").each(function () {
        var $parent = $(this).parent();
        $parent.append("<em class='is-error'></em>");
    });


    //trim text
    $(document).on("change", "#" + form + " input, #" + form + " textarea, #" + form + " select", function (event) {
        var data = $(this).val();
        var $current = $(this);
        if ($current.attr("type") != "file") {
            $current.val(data.trim());
        }
        var $parent = $current.parent();

        //validate number
        if ($current.hasClass("require-number")) {
            if (data != "" && data.length > 0) {
                var regex = /^[0-9]{1,10}$/;
                if (!regex.test(data)) {
                    $parent.find(".is-error").text("Bạn phải nhập số").addClass("show-error");
                    return;
                } else {
                    $parent.find(".is-error").removeClass("show-error");
                }
            }
        }
        //validate email
        if ($current.hasClass("require-email")) {
            if (data != "" && data.length > 0) {
                var regex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                if (!regex.test(data)) {
                    $parent.find(".is-error").text("Email không hợp lệ").addClass("show-error");
                    return;
                } else {
                    $parent.find(".is-error").removeClass("show-error");
                }
            }
        }

        //validate phone
        if ($current.hasClass("require-phone")) {
            if (data != "" && data.length > 0) {
                var regex = /^[0-9]{10,15}$/;
                if (!regex.test(data)) {
                    $parent.find(".is-error").text("Số điện thoại không đúng").addClass("show-error");
                    return;
                } else {
                    $parent.find(".is-error").removeClass("show-error");
                }
            }
        }

        //validate CMND
        if ($current.hasClass("require-cmnd")) {
            if (data != "" && data.length > 0) {
                var regex = /^([0-9]+{9} || [0-9]+{12})$/;
                if (!regex.test(data)) {
                    $parent.find(".is-error").text("Số CMND không đúng").addClass("show-error");
                    return;
                } else {
                    $parent.find(".is-error").removeClass("show-error");
                }
            }
        }

        //validate Date and Compare Date
        if ($current.hasClass("datepicker")) {
            if (data != "" && data.length > 0) {
                var date_regex = /^\d{1,2}\/\d{1,2}\/\d{4}$/;
                if (!date_regex.test(data)) {
                    $parent.find(".is-error").text("Ngày sai định dạng").addClass("show-error");
                    return;
                }
                var dateParts = data.split("/");
                if (dateParts.length != 3) {
                    $parent.find(".is-error").text("Ngày sai định dạng").addClass("show-error");
                    return;
                }
                var year = dateParts[2];
                var month = dateParts[1];
                var day = dateParts[0];

                if (isNaN(day) || isNaN(month) || isNaN(year) || month > 12 || day > 32) {
                    $parent.find(".is-error").text("Ngày sai định dạng").addClass("show-error");
                    return;
                }

                var dateResult = new Date(year, month, day);
                if (dateResult == null) {
                    $parent.find(".is-error").text("Ngày sai định dạng").addClass("show-error");
                    return;
                }
                $parent.find(".is-error").removeClass("show-error");
                //check compare min date vs max date
                var attrMaxDate = $current.attr("data-error-max-date");
                if (typeof (attrMaxDate) != 'undefined' && attrMaxDate != null) {
                    var $maxDate = $("#" + form + " input[name=" + attrMaxDate + "]");
                    if (typeof ($maxDate) != 'undefined' && $maxDate != null) {
                        var dataMaxDate = $maxDate.val();
                        if (dataMaxDate.length > 0) {
                            var dateSplit = dataMaxDate.split("/");
                            if (dateSplit.length == 3) {
                                var dateMax = new Date(dateSplit[2], dateSplit[1], dateSplit[0]);
                                if (dateMax != null) {
                                    if (dateResult >= dateMax) {
                                        $parent.find(".is-error").text("Ngày bắt đầu phải nhỏ hơn ngày kết thúc").addClass("show-error");
                                        return;
                                    } else {
                                        var $parentMaxDate = $maxDate.parent();
                                        $parentMaxDate.find(".is-error").removeClass("show-error");
                                    }
                                }
                            }
                        }
                    }
                }
                //check compare max date vs min date
                var attrMinDate = $current.attr("data-error-min-date");
                if (typeof (attrMinDate) != 'undefined' && attrMinDate != null) {
                    var $minDate = $("#" + form + " input[name=" + attrMinDate + "]");
                    if (typeof ($minDate) != 'undefined' && $minDate != null) {
                        var dataMinDate = $($minDate).val();
                        if (dataMinDate.length > 0) {
                            var dateSplit = dataMinDate.split("/");
                            if (dateSplit.length == 3) {
                                var dateMin = new Date(dateSplit[2], dateSplit[1], dateSplit[0]);
                                if (dateMin != null) {
                                    if (dateMin >= dateResult) {
                                        $parent.find(".is-error").text("Ngày kết thúc phải lớn hơn ngày bắt đầu").addClass("show-error");
                                        return;
                                    } else {
                                        var $parentMinDate = $minDate.parent();
                                        $parentMinDate.find(".is-error").removeClass("show-error");
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        //remove existed ajax server
        var error_existed = $current.attr("data-error-existed");
        if (typeof (error_existed) != 'undefined' && error_existed != '') {
            if (data != "" && data.length > 0) {
                var name = $current.attr("name").toString();
                var url = error_existed + "?" + name + "=" + data;
                $.ajax({
                    type: "post",
                    url: url,
                    cache: false,
                    dataType: 'json',
                    success: function (response) {
                        if (response.Status) {
                            $current.parent().find(".is-error").text("Dữ liệu đã tồn tại").addClass("show-error");
                            return;
                        } else {
                            $current.parent().find(".is-error").removeClass("show-error");
                        }
                    }, error: function (xhr) {
                        CommonJS.NotifyError("Đã có lỗi xảy ra");
                    }
                });
            }
        }




        //validate require input
        if ($current.hasClass("require")) {
            if (data == "" || data.length == 0) {
                $parent.find(".is-error").text("Trường dữ liệu không được để trống").addClass("show-error");
                return;
            } else {
                $parent.find(".is-error").removeClass("show-error");
            }
        }
    });

    $(document).on("change", "#" + form + " input[type=file]", function (event) {
        //validate file extension
        var $current = $(this);
        if ($current.hasClass("require-extension")) {
            var file_extension_default = "jpg|png|gif|doc|docx|xls|xlsx|zip|rar|ppt|pptx";
            var file_extension = $current.attr("data-extension");
            if (file_extension != null && file_extension.length > 0) {
                file_extension_default = file_extension;
            }

            var arrExtension = file_extension_default.split("|");
            if (arrExtension != null && arrExtension.length > 0) {
                var current_extension = $current.val().split(".");
                current_extension = current_extension[current_extension.length - 1];
                if (arrExtension.indexOf(current_extension) == -1) {
                    CommonJS.NotifyError("Định dạng file không đúng (định dạng cho phép " + file_extension_default + ")");
                    $current.val("");
                    return;
                }

            }
        }
    });
}
$(document).on("change", "input[type=text]", function () {
    if ($(this).val() != null && $(this).val().length > 0) {
        $(this).val($(this).val().trim());
    }
});