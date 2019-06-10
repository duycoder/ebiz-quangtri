//created by NAMDV
//Lấy dữ liệu Phường/Xã, Quận/Huyện tương ứng theo đơn vị cấp trên
function LoadDiaDiem(TINH_ID, HUYEN_ID, XA_ID, TYPE) {
    var url_loader = "/Common/LoadDiaDiem";
    // lấy id của tỉnh, huyện, xã đã được chọn
    var tinh_id = $("#" + TINH_ID).val();
    var huyen_id = $("#" + HUYEN_ID).val();
    var xa_id = $("#" + XA_ID).val();
    if (huyen_id.length == 0) {
        huyen_id = -1;
    }
    if (xa_id.length == 0) {
        xa_id = -1;
    }
    if (tinh_id.length >= 3) {
        $.ajax({
            url: url_loader,
            type: 'POST',
            data: { TINH_ID: tinh_id, HUYEN_ID: huyen_id, XA_ID: xa_id, TYPE: TYPE },
            success: function (data) {
                var items = "";
                if (TYPE == 1) {
                    items = "<option value='-1'>[-- Quận/Huyện --]</option>";
                } else if (TYPE == 2) {
                    items = "<option value='-1'>[-- Phường/Xã --]</option>";
                }
                $.each(data, function (i, type) {
                    //Tỉnh load Huyện
                    if (TYPE == 1) {
                        items += "<option value='" + type.HUYEN_ID + "'>" + type.TENHUYEN + "</option>";
                    }
                    //Huyện load xã
                    if (TYPE == 2) {
                        items += "<option value='" + type.XA_ID + "'>" + type.TENXA + "</option>";
                    }
                });
                if (TYPE == 1) {
                    $('#' + HUYEN_ID).html(items);
                    $('#' + XA_ID).html("<option value='-1'>[-- Phường/Xã --]</option>");
                }
                //Huyện load xã
                if (TYPE == 2) {
                    $('#' + XA_ID).html(items);
                }
            },
            error: function (data) {
                $('#' + HUYEN_ID).html("<option value='-1'>[-- Quận/Huyện --]</option>");
                $('#' + XA_ID).html("<option value='-1'>[-- Phường/Xã --]</option>");
            }
        });
    }
    else {
        $('#' + HUYEN_ID).html("<option value='-1'>[-- Quận/Huyện --]</option>");
        $('#' + XA_ID).html("<option value='-1'>[-- Phường/Xã --]</option>");
    }
}
//datepicker
function LoadDonvi(TINH_ID, DONVI_ID) {
    // lấy tỉnh, huyện, xã đã được chọn
    var tinh_id = $("#" + TINH_ID).val();
    var donvi_id = $("#" + DONVI_ID).val();
    var url_loader = "/Doituongtiem/LoadDonVi";
    // sử dụng ajax để load đơn vị
    $.ajax({
        url: url_loader,
        type: 'POST',
        data: { TINH_ID: tinh_id, DONVI_ID: donvi_id },
        success: function (data) {
            var items = "<option value='-1'>[--- Cơ sở ---]</option>";
            $.each(data, function (i, type) {
                items += "<option value='" + type.COSO_ID + "'>" + type.TENCOSO + "</option>";
            });
            $("#" + DONVI_ID).html(items);
        },
        error: function (data) {
            $("#" + DONVI_ID).html("<option value='-1'>[--- Cơ sở ---]</option>");
        }
    });
}
//load tỉnh huyện
function LoadDiaDiemTinh(TINH_ID, HUYEN_ID, TYPE) {
    var url_loader = "/Common/LoadDiaDiemTinh";
    // lấy id của tỉnh, huyện, xã đã được chọn
    var tinh_id = $("#" + TINH_ID).val();
    var huyen_id = $("#" + HUYEN_ID).val();
    if (tinh_id.length >= 3) {
        $.ajax({
            url: url_loader,
            type: 'POST',
            data: { TINH_ID: tinh_id, HUYEN_ID: huyen_id, TYPE: TYPE },
            success: function (data) {
                var items = "";
                if (TYPE == 1) {
                    items = "<option value='-1'>[-- Quận/Huyện --]</option>";
                }
                $.each(data, function (i, type) {
                    //Tỉnh load Huyện
                    if (TYPE == 1) {
                        items += "<option value='" + type.HUYEN_ID + "'>" + type.TENHUYEN + "</option>";
                    }
                });
                if (TYPE == 1) {
                    $('#' + HUYEN_ID).html(items);
                }
            },
            error: function (data) {
                $('#' + HUYEN_ID).html("<option value='-1'>[-- Quận/Huyện --]</option>");
            }
        });
    }
    else {
        $('#' + HUYEN_ID).html("<option value='-1'>[-- Quận/Huyện --]</option>");
    }
}
//Validate dữ liệu trước khi submit form
//element: phần tử cần validate
//element_error: phần tử hiển thị khi validate rỗng
//regex: biểu thức validate
//element_error_regex: phần tử hiển thị khi validate biểu thức
function validate_element(element, element_error, nameRegex, element_error_regex, isNumber) {
    //regex = /[0-9 -()+]+$/;
    //alert(element.attr("id"));
    if (typeof element != undefined) {
        if (element.val().trim().length <= 0) {
            element_error.show();
            element.focus();
            return 0;
        }
        else {
            if (isNumber == "1") {
                if (parseInt(element.val().trim()) <= 0) {
                    element_error.show();
                    element.focus();
                    return 0;
                }
                else {
                    element_error.hide();
                }
            } else {
                element_error.hide();
            }
        }
        return 1;
    }
    return 1;
}
function validate_element_regex(element, element_error, useRegex, nameRegex, element_error_regex, isNumber) {
    var validate_ele = validate_element(element, element_error, nameRegex, element_error_regex, isNumber);
    if (validate_ele == 1) {
        if (useRegex == "1") {
            var regex = new RegExp(nameRegex);
            if (!regex.test(element.val().trim())) {
                element_error_regex.show();
                element.focus();
                return 0;
            }
            else {
                element_error_regex.hide();
            }
        }
    }
    else {
        return 0;
    }
    return 1;
}
function validate_lenght(element, min, max, element_error) {
    if (element.attr("id") == "CMND") {
        if (min == 9 && max == 12) {
            if (element.val().trim().length != min && element.val().trim().length != max) {
                element_error.show();
                element.focus();
                return 0;
            }
            else {
                element_error.hide();
            }
        }
    }
    else {
        if (min > 0) {
            if (element.val().trim().length < min) {
                element_error.show();
                element.focus();
                return 0;
            }
            else {
                element_error.hide();
            }
        }
        else {
            element_error.hide();
        }
        if (max > 0) {
            if (element.val().trim().length > max) {
                element_error.show();
                element.focus();
                return 0;
            }
            else {
                element_error.hide();
            }
        }
        else {
            element_error.hide();
        }
    }
    return 1;
}
function validate_lenght_regex(element, min, max, element_error, useRegex, nameRegex, element_error_regex) {
    element_error_regex.hide(); element_error.show();
    var val_lenght = validate_lenght(element, min, max, element_error);
    if (val_lenght == 1) {
        if (useRegex == "1") {
            var regex = new RegExp(nameRegex);
            if (!regex.test(element.val().trim())) {
                element_error_regex.show();
                element.focus();
                return 0;
            }
            else {
                element_error_regex.hide();
            }
        }
    }
    else {
        return 0;
    }
    return 1;
}
function checkDateTime(date) {
    // regular expression to match required date format
    re = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/;
    if (date != '') {
        if (regs = date.match(re)) {
            // day value between 1 and 31
            if (regs[1] < 1 || regs[1] > 31) {
                return false;
            }
            // month value between 1 and 12
            if (regs[2] < 1 || regs[2] > 12) {
                return false;
            }
            if ((regs[1] > 29 && regs[2] == 2)) {
                return false;
            }
            // year value between 1895 and now
            if (regs[3] < 1895) {
                return false;
            }
            //try {
            //    var date1 = $.datepicker.parseDate("dd-mm-yy",date);
            //    //$("#SEARCH_BATDAU_ERROR").hide();
            //}
            //catch (err) {
            //   // $("#SEARCH_BATDAU_ERROR").show();
            //    return false;
            //}
        } else {
            return false;
        }
    }
    else {
        return false;
    }
    return true;
}
function checkMonthYear(date) {
    // regular expression to match required date format
    re = /^(\d{1,2})\-(\d{4})$/;
    if (date != '') {
        if (regs = date.match(re)) {
            // month value between 1 and 12
            if (regs[1] < 1 || regs[1] > 12) {
                return false;
            }
            // year value between 1895 and 2081
            if (regs[2] < 1951 && regs[2] > 2081) {
                return false;
            }
        } else {
            return false;
        }
    }
    else {
        return false;
    }
    return true;
}
function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}
function validatePhone(phone) {
    var re = /^[0-9]{10,11}$/;
    return re.test(phone);
}
function validateCMND(cmnd) {
    var re = /^[0-9]{9,12}$/;
    if (!re.test(cmnd)) {
        return false;
    }
    if (cmnd.length == 9 || cmnd.length == 12) {
        return true;
    }
    return false;
}
function removehtml(Html_string) {
    var isHTML = RegExp.prototype.test.bind(/(<([^>]+)>)/i);
    return isHTML(Html_string);
}
function deletehtml(Html_string) {
    return Html_string.replace(/(<([^>]+)>)/ig, "").trim();
}
function checkfile() {
    var so_file = $("#SO_FILE_CURRENT").val();
    var lst_so_file = so_file.split(',');
    var extension = $("#EXTENSION_HIDDEN").val();
    var lst_extension = extension.split(',');
    var file_size = $("#MAXSIZE_HIDDEN").val();
    var file_error = true;
    outloop:
        for (var i = 0; i < lst_so_file.length - 1; i++) {
            if (document.getElementById('filebase_' + lst_so_file[i]) != null) {
                if ($('#filebase_' + lst_so_file[i]).val() != "") {
                    var fsize = $('#filebase_' + lst_so_file[i])[0].files[0].size;
                    var ftype = $('#filebase_' + lst_so_file[i])[0].files[0].type;
                    var file = $('#filebase_' + lst_so_file[i]).val();
                    var extension = file.replace(/^.*\./, '');
                    innerloop:
                        for (var j = 0; j < lst_extension.length; j++) {
                            if (extension == lst_extension[j].split('.').pop() && fsize <= file_size) {
                                $("#file_null_" + lst_so_file[i]).hide();
                                file_error = true;
                                break innerloop;
                            } else {
                                $("#file_null_" + lst_so_file[i]).show();
                                //alert("ok");
                                file_error = false;
                            }
                        }
                }
            }
        }
    if (file_error == false) {
        return false;
    } else {
        return true;
    }
}
function getDateFromString(dateStr) {
    if (dateStr !== null && dateStr.trim() !== '') {
        var from = dateStr.trim().split("/");
        return new Date(from[2], from[1] - 1, from[0]);
    }
    return null;
}