function numbersonly(e) {
    var unicode = e.charCode ? e.charCode : e.keyCode;
    //s alert(unicode)
    if (unicode != 8 && unicode != 46 && unicode != 37 && unicode != 39 && unicode != 35 && unicode != 36 && unicode != 9) {
        if (unicode < 48 || unicode > 57)
            return false;
    }
};
function numberSoLuong(e) {
    var unicode = e.charCode ? e.charCode : e.keyCode;

    if (unicode != 8 && unicode != 37 && unicode != 39 && unicode != 35 && unicode != 36  && unicode != 9) {
        if (unicode < 48 || unicode > 57)
            return false;
    }
};

//hàm format tiền tệ
var format = function (num) {
    var str = num.toString(), parts = false, output = [], i = 1, formatted = null;
    if (str.indexOf(".") > 0) {
        parts = str.split(".");
        str = parts[0];
    }
    str = str.split("").reverse();
    for (var j = 0, len = str.length; j < len; j++) {
        if (str[j] != ",") {
            output.push(str[j]);
            if (i % 3 == 0 && j < (len - 1)) {
                output.push(",");
            }
            i++;
        }
    }
    formatted = output.reverse().join("");
    return (formatted + ((parts) ? "." + parts[1].substr(0, 2) : ""));
};

//hàm keyup tiến hành thực hiện format tiền tệ
function FormatCurrency(obj) {
    var id = obj.getAttribute("id");
    if ($("#" + id + "").val() != format($("#" + id + "").val())) {
        $("#" + id + "").val(format($("#" + id + "").val()));
    }


    //test đọc tiền
    var id_hidden = id.substring(0, id.length - 5);
    $("#" + id + "").val(format($("#" + id + "").val()));
    var hiden_val = $("#" + id + "").val().replace(/,/gi, "");
    if (hiden_val > 0) {
        $("#" + id_hidden + "_DOC").css('display', 'inline');
        $("#" + id_hidden + "_DOC").html("(" + DocTienBangChu(hiden_val) + " đồng Việt Nam)");
    } else {
        $("#" + id_hidden + "_DOC").css('display', 'none');
        $("#" + id_hidden + "_DOC").html("");
    }

    //end test đọc tiền



    //test
    var _check = $("#" + id + "").val().charAt(0);
    //console.log(_check);
    if ($("#" + id + "").val().indexOf("0") == 0) {
        return false;
    }
    if (_check == ",") {
        var test_last = $("#" + id + "").val();
        var test_after = test_last.substring(1, test_last.length);
        $("#" + id + "").val(test_after);
    }
    //if (_check == "0") {
    //    //console.log(2);
    //    if ($("#" + id + "").val().trim().length == 3) {
    //        var test_last = $("#" + id + "").val();
    //        var test_after = test_last.substring(1, test_last.length);
    //        $("#" + id + "").val(test_after);
    //        console.log(test_last);
    //        console.log(test_after);
    //    }
    //    else {
    //        //var test_last = $("#" + id + "").val();
    //       // var test_after = $("#" + id + "").val().substring(2, $("#" + id + "").val().length);
    //        $("#" + id + "").val($("#" + id + "").val().substring(2, $("#" + id + "").val().length));
    //        //console.log(test_last);
    //        //console.log(test_after);
    //    }

    //}
    //entest

};

//hàm focusout đẩy giá trị tiền tệ xuống hidden
function AssignValue(obj) {
    var id = obj.getAttribute("id");
    var id_hidden = id.substring(0, id.length - 5);
    $("#" + id + "").val(format($("#" + id + "").val()));
    var hiden_val = $("#" + id + "").val().replace(/,/gi, "");
    $("#" + id_hidden + "").val(hiden_val);
    //console.log(id.substring(0,id.length-5));
};

//function FormatCurrency(obj) {
//    var id = obj.getAttribute("id");
//    if ($("#" + id + "").val() != format($("#" + id + "").val())) {
//        $("#" + id + "").val(format($("#" + id + "").val()));
//    }
//};

//hàm check giá trị tỉ lệ chiết khấu, lãi suất
function CheckPercent(obj) {
    var id = obj.getAttribute("id");
    if ($("#" + id + "").val() > 100) {
        $("#CHECK_VALUE_" + id + "").css('display', 'inline');
    } else {
        $("#CHECK_VALUE_" + id + "").css('display', 'none');
    }
}

//đọc tiền
function ChuyenSo(number) { var doc = 1;
    var dv = ['', 'mươi', 'trăm', 'nghìn', 'triệu', 'tỉ'];
    var cs = ['không', 'một', 'hai', 'ba', 'bốn', 'năm', 'sáu', 'bảy', 'tám', 'chín'];
    var doc;
    var i, j, k, n, len, found, ddv, rd;

    len = number.Length;
    number += "ss";
    doc = "";
    found = 0;
    ddv = 0;
    rd = 0;

    i = 0;
    while (i < len) {
        //So chu so o hang dang duyet
        n = (len - i + 2) % 3 + 1;

        //Kiem tra so 0
        found = 0;
        for (j = 0; j < n; j++) {
            if (number[i + j] != '0') {
                found = 1;
                break;
            }
        }

        //Duyet n chu so
        if (found == 1) {
            rd = 1;
            for (j = 0; j < n; j++) {
                ddv = 1;
                switch (number[i + j]) {
                    case '0':
                        if (n - j == 3) doc += cs[0] + " ";
                        if (n - j == 2) {
                            if (number[i + j + 1] != '0') doc += "lẻ ";
                            ddv = 0;
                        }
                        break;
                    case '1':
                        if (n - j == 3) doc += cs[1] + " ";
                        if (n - j == 2) {
                            doc += "mười ";
                            ddv = 0;
                        }
                        if (n - j == 1) {
                            if (i + j == 0) k = 0;
                            else k = i + j - 1;

                            if (number[k] != '1' && number[k] != '0')
                                doc += "mốt ";
                            else
                                doc += cs[1] + " ";
                        }
                        break;
                    case '5':
                        if (i + j == len - 1)
                            doc += "lăm ";
                        else
                            doc += cs[5] + " ";
                        break;
                    default:
                        doc += cs[parseInt(number[i + j]) - 48] + " ";
                        break;
                }

                //Doc don vi nho
                if (ddv == 1) {
                    doc += dv[n - j - 1] + " ";
                }
            }
        }


        //Doc don vi lon
        if (len - i - n > 0) {
            if ((len - i - n) % 9 == 0) {
                if (rd == 1)
                    for (k = 0; k < (len - i - n) / 9; k++)
                        doc += "tỉ ";
                rd = 0;
            }
            else
                if (found != 0) doc += dv[((len - i - n + 1) % 9) / 3 + 2] + " ";
        }

        i += n;
    }

    if (len == 1)
        if (number[0] == '0' || number[0] == '5') return cs[parseInt(number[0]) - 48];
   
    return doc;
}


//tesst

var ChuSo = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
var Tien = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");

//1. Hàm đọc số có ba chữ số;
function DocSo3ChuSo(baso) {
    var tram;
    var chuc;
    var donvi;
    var KetQua = "";
    tram = parseInt(baso / 100);
    chuc = parseInt((baso % 100) / 10);
    donvi = baso % 10;
    if (tram == 0 && chuc == 0 && donvi == 0) return "";
    if (tram != 0) {
        KetQua += ChuSo[tram] + " trăm ";
        if ((chuc == 0) && (donvi != 0)) KetQua += " linh ";
    }
    if ((chuc != 0) && (chuc != 1)) {
        KetQua += ChuSo[chuc] + " mươi";
        if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh ";
    }
    if (chuc == 1) KetQua += " mười ";
    switch (donvi) {
        case 1:
            if ((chuc != 0) && (chuc != 1)) {
                KetQua += " mốt ";
            }
            else {
                KetQua += ChuSo[donvi];
            }
            break;
        case 5:
            if (chuc == 0) {
                KetQua += ChuSo[donvi];
            }
            else {
                KetQua += " lăm ";
            }
            break;
        default:
            if (donvi != 0) {
                KetQua += ChuSo[donvi];
            }
            break;
    }
    return KetQua;
}

//2. Hàm đọc số thành chữ (Sử dụng hàm đọc số có ba chữ số)

function DocTienBangChu(SoTien) {
    var lan = 0;
    var i = 0;
    var so = 0;
    var KetQua = "";
    var tmp = "";
    var ViTri = new Array();
    if (SoTien < 0) return "Số tiền âm !";
    if (SoTien == 0) return "Không đồng !";
    if (SoTien > 0) {
        so = SoTien;
    }
    else {
        so = -SoTien;
    }
    if (SoTien > 8999999999999999) {
        //SoTien = 0;
        return "Số quá lớn!";
    }
    ViTri[5] = Math.floor(so / 1000000000000000);
    if (isNaN(ViTri[5]))
        ViTri[5] = "0";
    so = so - parseFloat(ViTri[5].toString()) * 1000000000000000;
    ViTri[4] = Math.floor(so / 1000000000000);
    if (isNaN(ViTri[4]))
        ViTri[4] = "0";
    so = so - parseFloat(ViTri[4].toString()) * 1000000000000;
    ViTri[3] = Math.floor(so / 1000000000);
    if (isNaN(ViTri[3]))
        ViTri[3] = "0";
    so = so - parseFloat(ViTri[3].toString()) * 1000000000;
    ViTri[2] = parseInt(so / 1000000);
    if (isNaN(ViTri[2]))
        ViTri[2] = "0";
    ViTri[1] = parseInt((so % 1000000) / 1000);
    if (isNaN(ViTri[1]))
        ViTri[1] = "0";
    ViTri[0] = parseInt(so % 1000);
    if (isNaN(ViTri[0]))
        ViTri[0] = "0";
    if (ViTri[5] > 0) {
        lan = 5;
    }
    else if (ViTri[4] > 0) {
        lan = 4;
    }
    else if (ViTri[3] > 0) {
        lan = 3;
    }
    else if (ViTri[2] > 0) {
        lan = 2;
    }
    else if (ViTri[1] > 0) {
        lan = 1;
    }
    else {
        lan = 0;
    }
    for (i = lan; i >= 0; i--) {
        tmp = DocSo3ChuSo(ViTri[i]);
        KetQua += tmp;
        if (ViTri[i] > 0) KetQua += Tien[i];
        if ((i > 0) && (tmp.length > 0)) KetQua += ',';//&& (!string.IsNullOrEmpty(tmp))
    }
    if (KetQua.substring(KetQua.length - 1) == ',') {
        KetQua = KetQua.substring(0, KetQua.length - 1);
    }
    KetQua = KetQua.substring(1, 2).toUpperCase() + KetQua.substring(2);
    return KetQua;//.substring(0, 1);//.toUpperCase();// + KetQua.substring(1);
}
//end tesst