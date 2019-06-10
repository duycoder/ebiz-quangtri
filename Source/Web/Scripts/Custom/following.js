function LoadDataByID(ParentID) {
    if ($("#F_" + ParentID).position() != undefined) {
        var left = $("#F_" + ParentID).position().left;
        if ($("#F_" + ParentID).position().left == 0) {
            left = 0;
        }
        var html = '<a href="javascript:void(0)" data-pid="' + ParentID + '" class="folder_arrow expand" style="left: ' + left + 'px;"></a>';
        if ($("#folder_" + ParentID).find(".folder_arrow").html() == undefined) {
            $("#folder_" + ParentID).before(html);
        }
        $("#F_" + ParentID).find("ul").remove();
    }
    var this1 = null;
    $("#Path li:last-child span.folder").click();
}
$(document).on('click', '.hinet-table .folder,.hinet-table .folder_root, .hinet-table .folder-left,.hinet-table .folder-right,.folder-read,.folder-write', function () {
    var this1 = $(this);
    this1.closest("li").find("ul").remove("ul");
    var isLoaded = $(this).attr('data-loaded');
    var id = $(this1).attr('data-pid');
    $("#SEARCH_FOLDER_ID").val(id);
    if (id == 0) {
        $(".hinet-table tfoot").remove();
        reloadTable();
        $("#URLPath #Path").html("");
        GetPath(0, "");
        $("#THEMTHUMUC").attr("data-pid", "0");
        $("#THEMTAILIEU").hide();
    } else {
        this1.attr("data-loaded", true);
        if ($(this1).closest("li").find(".folder-item").length > 0) {
            loadDataById(this1, false);
        } else {
            if ($(this1).closest("li").find(".folder_arrow").length > 0) {
                $(this1).closest("li").find(".folder_arrow").click();
            } else {
                loadDataById(this1, false);
            }
        }
    }
});
function loadDataById(this1, isLiveId) {
    var pid = isLiveId ? this1 : $(this1).attr('data-pid');
    $(".folder").removeClass("text-bold");
    $(this1).closest("li").find(".folder").addClass("text-bold");
    $(this1).closest("li").removeClass('collapsed').addClass('expanded');
    $(".hinet-table tbody").html("");
    $(".hinet-table tfoot").html("");
    $.ajax({
        url: '/THUMUCLUUTRUArea/Following/GetChild',
        type: "GET",
        data: {
            pid: pid,
            sort: $(this1).attr('data-sort')
        },
        dataType: "json",
        beforeSend: function () {
            $(".loading-ajax").show();
        },
        success: function (d) {
            $("#THUMUC_ID_HIDDEN").val($(this1).attr('data-pid'));
            $("#THEMTHUMUC").attr("data-pid", $(this1).attr('data-pid'));
            $("#THEMTAILIEU").attr("data-pid", $(this1).attr('data-pid'));
            $("#THEMTAILIEU").show();
            $(".THEMTHUMUC").show();
            GetPath($(this1).attr('data-pid'), $(this1).text());
            var left = 0;
            if ($("#F_" + $(this1).attr('data-pid')).position() != undefined) {
                left = $("#F_" + $(this1).attr('data-pid')).position().left + 16;
                $(this1).css("left", $("#F_" + $(this1).attr('data-pid')).position().left);
            }
            if (d.length > 0) {
                $(".hntbl-counter").html(d.length);
                var $ul = $("<ul></ul>");
                var count = 0;
                var thumuc = "";
                var tailieu = "";
                $.each(d, function (i, ele) {
                    if (ele.PERMISSION == parseInt($("#CAN_READ").val())) {
                        $("#THEMTAILIEU").hide();
                        $(".THEMTHUMUC").hide();
                    }
                    if (ele.IS_THUMUC) {
                        var thumuc_trangthai = "";
                        if (ele.SLTHUMUC > 0) {
                            thumuc_trangthai += '<a style="left:' + left + 'px" data-pid="' + ele.ID + '" href="javascript:void(0)" class="folder_arrow"></a>';
                        }
                        thumuc_trangthai += "<span class='folder allsystem1' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";
                        $ul.append(
                            $("<li data-fid='F_" + ele.ID + "' class='collapsed' id='F_" + ele.ID + "'></li>").append(
                                thumuc_trangthai
                            )
                        )
                    }
                    var ngaytao = ToDate(ele.NGAYTAO);
                    if (ele.IS_THUMUC) {
                        var thumuc_trangthai = "";
                        thumuc_trangthai = "<span data-per='" + ele.PERMISSION + "' data-sort='0' class='folder folder-common allsystem' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";;
                        var share = "";
                        thumuc += "<tr  class='folder-item'>";
                        thumuc += "<td></td>";
                        thumuc += "<td data-fid='F_" + ele.ID + "'><div style='width:100%;float:left;margin-top:-6px !important' class='TENTHUMUC'>" + thumuc_trangthai + "</div></td>";
                        thumuc += "<td>" + ele.TEN_DONVI + "</td>";
                        thumuc += "<td>" + ele.TEN_NGUOITAO + "</td>";
                        thumuc += "<td>" + ngaytao + "</td>";
                        thumuc += "<td>" + initContext(ele.ID, true) + "</td>";
                        thumuc += "</tr>";
                    } else {
                        var extension = ele.THUMUCCHA;
                        var docx = "";
                        if (extension.split('/')[1] == "vnd.openxmlformats-officedocument.wordprocessingml.document" || extension == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || extension == "vnd.openxmlformats-officedocument.wordprocessingml.document") {
                            docx = "msword";
                        } else if (extension == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                            docx = "ms-excel";
                        } else if (extension.split('/')[1] == "vnd.openxmlformats-officedocument.presentationml.presentation") {
                            docx = "ms-powerpoint";
                        }
                        tailieu += "<tr  class='file-item'>";
                        tailieu += "<td></td>";
                        tailieu += "<td data-fid='F_" + ele.ID + "'><span data-per='" + ele.PERMISSION + "' data-sort='0' class='" + extension.split('/')[1] + " " + docx + " " + extension.split('.')[1] + " public-files' data-file='true' data-pid='" + ele.ID + "'>" + htmlDecode(ele.TENTHUMUC) + "</span></td>";
                        tailieu += "<td>" + ele.TEN_DONVI + "</td>";
                        tailieu += "<td>" + ele.TEN_NGUOITAO + "</td>";
                        tailieu += "<td>" + ngaytao + "</td>";
                        tailieu += "<td>" + initContext(ele.ID, false) + "</td>";
                        tailieu += "</tr>";
                    }
                });
                if ($(".hinet-table tfoot").length == 0) {
                    $(".hinet-table").append("<tfoot></tfoot>");
                }
                $(".hinet-table tbody").append(thumuc);
                $(".hinet-table tfoot").append(tailieu);
                count = 0;
                reloadIndex();
                $(this1).parent().append($ul);
                $(this1).toggleClass('expand');
                $(this1).closest('li').children('ul').slideDown();
            } else {
                if (parseInt($(this1).attr('data-per')) == parseInt($("#CAN_READ").val())) {
                    $("#THEMTAILIEU").hide();
                    $("#THEMTHUMUC").hide();
                }
                $(".hntbl-counter").html($(".hinet-table tbody tr").length + $(".hinet-table tfoot tr").length);
                $(".hinet-table tbody").append("<tr><td colspan='6' class='error'>Không có dữ liệu</td></tr>");
            }
        },
        error: function (xhr, response) {
            alert(xhr.responseText);
        }, complete: function () {
            $(".loading-ajax").hide();
        }
    });
}
function reloadIndex() {
    var count = 0;
    $(".hinet-table tbody tr").each(function () {
        count++;
        $(this).find("td:first-child").html(count);
    });
    $(".hinet-table tfoot tr").each(function () {
        count++;
        $(this).find("td:first-child").html(count);
    });
    $(".hntbl-counter").html(count);
}
function GetPath(pid, TENTHUMUC) {
    if (pid > 0) {
        $("#Path").append($("<li data-fid='F_" + pid + "'></li>").append(
            "<span class='folder' data-loaded='false' data-pid='" + pid + "'><i class=\"fa fa-folder-open-o\"></i>" + TENTHUMUC + "<i class=\"fa fa-chevron-right\"></i></span>"
        )
        )
    } else {
        $("#Path").html($("<li data-fid='F_0'></li>").append(
            "<span class='folder' data-loaded='false' data-pid='0'><i class=\"fa fa-folder-open-o\"></i>Gốc<i class=\"fa fa-chevron-right\"></i></span>"
        )
        )
    }

}
function GetFullPath(pid) {
    $.ajax({
        url: '/THUMUCLUUTRUArea/Following/GetUrlBar',
        type: 'GET',
        data: {
            pID: pid
        },
        dataType: "json",
        success: function (d) {
            $("#DUONGDAN").html("");
            var $ul = $("<ul id='URL_PATH'></ul>");
            $ul.append($("<li data-fid='F_0'></li>").append(
                "<span><i class=\"fa fa-folder-open-o url-folder\"></i>Gốc<i class=\"fa fa-chevron-right\"></i></span>"
            )
            )
            if (d.length > 1) {
                for (var i = d.length - 1; i >= 0; i--) {
                    if (i == 0) {
                        $ul.append(
                            $("<li data-fid='F_" + d[i].ID + "'></li>").append(
                                "<span><i class=\"fa fa-folder-open-o url-folder\"></i>" + d[i].TENTHUMUC + "</span>"
                            )
                        )
                    } else {
                        $ul.append(
                            $("<li data-fid='F_" + d[i].ID + "'></li>").append(
                                "<span><i class=\"fa fa-folder-open-o url-folder\"></i>" + d[i].TENTHUMUC + "<i class=\"fa fa-chevron-right\"></i></span>"
                            )
                        )
                    }
                }
            } else if (d[0] != null) {
                $ul.append(
                    $("<li data-fid='F_" + d[0].ID + "'></li>").append(
                        "<i class=\"fa fa-folder-open-o url-folder\"></i><span>" + d[0].TENTHUMUC + "</span>"
                    )
                )
            } else {
                $ul.append("");
            }
            $("#DUONGDAN").append($ul);
        },
        error: function (xhr) {
            CommonJS.alert(xhr.responseText);
        }
    });
}
function htmlDecode(html) {
    return html.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}
function initContext(data) {
    var id = data.ID;
    var html = '<div class="dropdown">';
    html += '<button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
    html += 'Thao tác';
    html += '</button>';
    html += '<div class="dropdown-menu" aria-labelledby="dropdownMenuButton">';
    if (data.IS_THUMUC) {
        if (data.PERMISSION == parseInt($("#CAN_WRITE").val())) {
            html += '<a class="dropdown-item" onclick="UploadSingle(' + id + ')" href="javascript:void(0)"><i class="fa fa-cloud-upload"></i> Upload 1 file</a>';
            html += '<a class="dropdown-item" onclick="UploadFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-upload"></i> Upload nhiều file (bằng cách chọn từng file)</a>';
            html += '<a class="dropdown-item" onclick="UploadZipFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-file-archive-o"></i> Upload file Zip/Rar và giải nén</a>';
            html += '<a class="dropdown-item" onclick="DragDrop(' + id + ')" href="javascript:void(0)"><i class="fa fa-icon-dropbox"></i> Upload nhiều file (bằng cách chọn nhiều)</a>';
            html += '<a class="dropdown-item" onclick="CreateThuMuc(' + id + ',false)" href="javascript:void(0)"><i class="fa fa-folder-open-o"></i> Thêm thư mục</a>';
            html += '<a class="dropdown-item" onclick="CreateThuMuc(' + id + ',true)" href="javascript:void(0)"><i class="fa fa-edit"></i> Cập nhật</a>';
            //html += '<a class="dropdown-item" onclick="SharingFolder(' + id + ',true)" href="javascript:void(0)"><i class="fa-share-alt"></i> Chia sẻ</a>';
        } else if (data.PERMISSION == parseInt($("#CAN_READ").val())) {
            html += '<a class="dropdown-item" href="/ThuMucLuuTruArea/ThuMucLuuTru/DownloadZipFile?id=' + id + '"><i class="fa fa-download"></i> Download dạng ZIP</a>';
        } else {
            html += '<a class="dropdown-item" href="/ThuMucLuuTruArea/ThuMucLuuTru/DownloadZipFile?id=' + id + '"><i class="fa fa-download"></i> Download dạng ZIP</a>';
            html += '<a class="dropdown-item" onclick="UploadSingle(' + id + ')" href="javascript:void(0)"><i class="fa fa-cloud-upload"></i> Upload 1 file</a>';
            html += '<a class="dropdown-item" onclick="UploadFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-upload"></i> Upload nhiều file (bằng cách chọn từng file)</a>';
            html += '<a class="dropdown-item" onclick="UploadZipFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-file-archive-o"></i> Upload file Zip/Rar và giải nén</a>';
            html += '<a class="dropdown-item" onclick="DragDrop(' + id + ')" href="javascript:void(0)"><i class="fa fa-icon-dropbox"></i> Upload nhiều file (bằng cách chọn nhiều)</a>';
            html += '<a class="dropdown-item" onclick="CreateThuMuc(' + id + ',false)" href="javascript:void(0)"><i class="fa fa-folder-open-o"></i> Thêm thư mục</a>';
            html += '<a class="dropdown-item" onclick="CreateThuMuc(' + id + ',true)" href="javascript:void(0)"><i class="fa fa-edit"></i> Cập nhật</a>';
            //html += '<a class="dropdown-item" onclick="SharingFolder(' + id + ',true)" href="javascript:void(0)"><i class="fa-share-alt"></i> Chia sẻ</a>';
        }
    } else {
        if (data.PERMISSION == parseInt($("#CAN_READ").val())) {
            html += '<a class="dropdown-item" onclick="DownloadFileIndex(' + id + ',null)" href="javascript:void(0)"><i class="fa fa-download"></i> Tải xuống</a>';
            html += '<a class="dropdown-item" onclick="ViewDetail(' + id + ')" href="javascript:void(0)"><i class="fa fa-eye"></i> Chi tiết</a>';

        } else if (data.PERMISSION == parseInt($("#CAN_WRITE").val())) {
            html += '<a class="dropdown-item" onclick="DoiTenFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-edit"></i> Đổi tên</a>';
            html += '<a class="dropdown-item" onclick="EditFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-pencil-square-o"></i> Chỉnh sửa</a>';
            html += '<a class="dropdown-item" onclick="ManagerVersion(' + id + ')" href="javascript:void(0)"><i class="fa fa-archive"></i> Quản lý phiên bản</a>';
        } else {
            html += '<a class="dropdown-item" onclick="DownloadFileIndex(' + id + ',null)" href="javascript:void(0)"><i class="fa fa-download"></i> Tải xuống</a>';
            //html += '<a class="dropdown-item" onclick="DoiTenFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-edit"></i> Đổi tên</a>';
            //html += '<a class="dropdown-item" onclick="SharingFile(' + id + ')" href="javascript:void(0)"><i class="fa-share-alt"></i> Chia sẻ</a>';
            html += '<a class="dropdown-item" onclick="ViewDetail(' + id + ')" href="javascript:void(0)"><i class="fa fa-eye"></i> Chi tiết</a>';
            //html += '<a class="dropdown-item" onclick="EditFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-pencil-square-o"></i> Chỉnh sửa</a>';
            //html += '<a class="dropdown-item" onclick="ManagerVersion(' + id + ')" href="javascript:void(0)"><i class="fa fa-archive"></i> Quản lý phiên bản</a>';
        }
    }
    html += '</div>';
    html += '</div>';
    return html;
}
function EditFile(id) {
    FileDetail(id, "chinhsua");
}
function ViewDetail(id) {
    FileDetail(id, "chitiet");
}
function DownloadFileIndex(ID, gui) {
    $.ajax({
        type: "POST",
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/CheckkingFile',
        data: {
            ID: ID
        },
        cache: false,
        dataType: "json",
        success: function (data) {
            //alert(data);
            if (data == "Co") {
                location.href = "/ThuMucLuuTruArea/ThuMucLuuTru/DownloadFile?ID=" + ID;
            } else {
                $.confirm({
                    'title': 'Không tìm thấy tài liệu',
                    'message': 'Xin lỗi không thể tìm thấy tài liệu mà bạn đang yêu cầu.',
                    'buttons': {
                        'Đóng': {
                            'class': 'btn-warning',
                            'action': function () { } // Nothing to do in this case. You can as well omit the action property.
                        }
                    }
                });
            }
        }
    });
}
function FileDetail(ID, OPTION) {
    $.ajax({
        type: "POST",
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/FileDetail',
        data: {
            TAILIEU: ID,
            OPTION: OPTION
        },
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#CreateThuMuc").html(data);
            $("#CreateThuMuc").modal('show');
            $("#CreateThuMuc").find(".modal-dialog").css("width", "900px");
        }, error: function (err) {
            console.log(err);
        }
    });
}
$(document).on("click", "#Path .folder,#Path .folder_root", function () {
    var id = $(this).attr('data-pid');
    $("#SEARCH_FOLDER_ID").val(id);
    if (id == 0) {
        $(".hinet-table tfoot").remove();
        reloadTable();
        $("#URLPath #Path").html("");
        GetPath(0, "");
        $("#THEMTHUMUC").attr("data-pid", "0");
        $("#THEMTAILIEU").hide();
        $("#THEMTHUMUC").show();
    } else {
        $("#THUMUC_ID_HIDDEN").val($(this1).attr('data-pid'));
        $("#THEMTHUMUC").attr("data-pid", $(this1).attr('data-pid'));
        $("#THEMTAILIEU").attr("data-pid", $(this1).attr('data-pid'));
        $("#THEMTAILIEU").show();
        $(".THEMTHUMUC").show();
        var index = $(this).closest("li").index();
        $("#Path li:nth-child(" + (index + 1) + ")").nextAll().remove();
        var pid = id;
        $(".folder").removeClass("text-bold");
        var this1 = $(this);
        $(this1).closest("li").find(".folder").addClass("text-bold");
        $(this1).closest("li").removeClass('collapsed').addClass('expanded');
        $(".hinet-table tbody").html("");
        $(".hinet-table tfoot").html("");
        $.ajax({
            url: '/THUMUCLUUTRUArea/Following/GetChild',
            type: "GET",
            data: {
                pid: pid,
                sort: $(this1).attr('data-sort')
            },
            dataType: "json",
            beforeSend: function () {
                $(".loading-ajax").show();
            },
            success: function (d) {
                var left = 0;
                if ($("#F_" + $(this1).attr('data-pid')).position() != undefined) {
                    left = $("#F_" + $(this1).attr('data-pid')).position().left + 16;
                    $(this1).css("left", $("#F_" + $(this1).attr('data-pid')).position().left);
                }
                if (d.length > 0) {
                    $(".hntbl-counter").html(d.length);
                    var count = 0;
                    var thumuc = "";
                    var tailieu = "";
                    $.each(d, function (i, ele) {
                        console.log(ele.PERMISSION);
                        if (ele.PERMISSION == parseInt($("#CAN_READ").val())) {
                            $("#THEMTAILIEU").hide();
                            $(".THEMTHUMUC").hide();
                        }
                        if (ele.IS_THUMUC) {
                            var thumuc_trangthai = "";
                            if (ele.SLTHUMUC > 0) {
                                thumuc_trangthai += '<a style="left:' + left + 'px" data-pid="' + ele.ID + '" href="javascript:void(0)" class="folder_arrow"></a>';
                            }
                            thumuc_trangthai += "<span class='folder allsystem1' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";
                        }
                        var ngaytao = ToDate(ele.NGAYTAO);
                        if (ele.IS_THUMUC) {
                            var thumuc_trangthai = "";
                            thumuc_trangthai = "<span data-sort='0' class='folder folder-common allsystem' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";;
                            var share = "";
                            thumuc += "<tr  class='folder-item'>";
                            thumuc += "<td></td>";
                            thumuc += "<td data-fid='F_" + ele.ID + "'><div style='width:100%;float:left' class='TENTHUMUC'>" + thumuc_trangthai + "</div></td>";
                            thumuc += "<td>" + ele.TEN_DONVI + "</td>";
                            thumuc += "<td>" + ele.TEN_NGUOITAO + "</td>";
                            thumuc += "<td>" + ngaytao + "</td>";
                            thumuc += "<td>" + initContext(ele.ID, true) + "</td>";
                            thumuc += "</tr>";
                        } else {
                            var extension = ele.THUMUCCHA;
                            var docx = "";
                            if (extension.split('/')[1] == "vnd.openxmlformats-officedocument.wordprocessingml.document" || extension == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || extension == "vnd.openxmlformats-officedocument.wordprocessingml.document") {
                                docx = "msword";
                            } else if (extension == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                                docx = "ms-excel";
                            } else if (extension.split('/')[1] == "vnd.openxmlformats-officedocument.presentationml.presentation") {
                                docx = "ms-powerpoint";
                            }
                            tailieu += "<tr  class='file-item'>";
                            tailieu += "<td></td>";
                            tailieu += "<td data-fid='F_" + ele.ID + "'><span data-sort='0' class='" + extension.split('/')[1] + " " + docx + " " + extension.split('.')[1] + " public-files' data-file='true' data-pid='" + ele.ID + "'>" + htmlDecode(ele.TENTHUMUC) + "</span></td>";
                            tailieu += "<td>" + ele.TEN_DONVI + "</td>";
                            tailieu += "<td>" + ele.TEN_NGUOITAO + "</td>";
                            tailieu += "<td>" + ngaytao + "</td>";
                            tailieu += "<td>" + initContext(ele.ID, false) + "</td>";
                            tailieu += "</tr>";
                        }
                    });
                    if ($(".hinet-table tfoot").length == 0) {
                        $(".hinet-table").append("<tfoot></tfoot>");
                    }
                    $(".hinet-table tbody").append(thumuc);
                    $(".hinet-table tfoot").append(tailieu);
                    count = 0;
                    reloadIndex();
                } else {
                    if (parseInt($(this).attr('data-per')) == parseInt($("#CAN_READ").val())) {
                        $("#THEMTAILIEU").hide();
                        $("#THEMTHUMUC").hide();
                    }
                    $(".hntbl-counter").html($(".hinet-table tbody tr").length + $(".hinet-table tfoot tr").length);
                    $(".hinet-table tbody").append("<tr><td colspan='6' class='error'>Không có dữ liệu</td></tr>");
                }
            },
            error: function (xhr, response) {
                alert(xhr.responseText);
            }, complete: function () {
                $(".loading-ajax").hide();
            }
        });
    }
});
function UploadSingle(ID) {
    $.ajax({
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/UploadFileSingle',
        type: 'post',
        cache: false,
        data: {
            FOLDER_ID: ID
        },
        success: function (data) {
            if (data.trim().length > 0) {
                $("#CreateThuMuc").html(data);
                $("#CreateThuMuc").modal('show');
                $("#CreateThuMuc").find(".modal-dialog").css("width", "900px");
            } else {
                $.confirm({
                    'title': 'Không thể tải lên tài liệu',
                    'message': 'Thư mục này đang chờ duyệt hoặc đã hết hạn.Vui lòng thử lại vào lúc khác.',
                    'buttons': {
                        'Đóng': {
                            'class': 'btn-confirm-yes',
                            'action': function () {
                            }
                        }
                    }
                });
            }
        },
        error: function (xhr) {
            CommonJS.alert(xhr.responseText);
        }
    });
}
function UploadFile(ID) {
    $.ajax({
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/UploadFile',
        type: 'post',
        cache: false,
        data: {
            FOLDER_ID: ID
        },
        success: function (data) {
            if (data.trim().length > 0) {
                $("#CreateThuMuc").html(data);
                $("#CreateThuMuc").modal('show');
                $("#CreateThuMuc").find(".modal-dialog").css("width", "900px");
                $("#CreateThuMuc").find(".modal-dialog").css("height", "450px");
            } else {
                $.confirm({
                    'title': 'Không thể tải lên tài liệu',
                    'message': 'Thư mục không tồn tại.',
                    'buttons': {
                        'Đóng': {
                            'class': 'btn-confirm-yes',
                            'action': function () {
                            }
                        }
                    }
                });
            }
        },
        error: function (xhr) {
            CommonJS.alert(xhr.responseText);
        }
    });
}
function UploadZipFile(ID) {
    $.ajax({
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/UploadZipFile',
        type: 'post',
        cache: false,
        data: {
            folderId: ID
        },
        success: function (data) {
            if (data.trim().length > 0) {
                $("#CreateThuMuc").html(data);
                $("#CreateThuMuc").modal('show');
                $("#CreateThuMuc").find(".modal-dialog").css("width", "750px");
                $("#CreateThuMuc").find(".modal-dialog").css("height", "550px");
            } else {
                $.confirm({
                    'title': 'Không thể tải lên tài liệu',
                    'message': 'Thư mục không tồn tại.',
                    'buttons': {
                        'Đóng': {
                            'class': 'btn-confirm-yes',
                            'action': function () {
                            }
                        }
                    }
                });
            }
        },
        error: function (xhr) {
            CommonJS.alert(xhr.responseText);
        }
    });
}
function DragDrop(ID) {
    $.ajax({
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/DragDropFile',
        type: 'post',
        cache: false,
        data: {
            folderId: ID
        },
        success: function (data) {
            if (data.trim().length > 0) {
                $("#CreateThuMuc").html(data);
                $("#CreateThuMuc").modal('show');
                $("#CreateThuMuc").find(".modal-dialog").css("width", "750px");
                $("#CreateThuMuc").find(".modal-dialog").css("height", "550px");
            } else {
                $.confirm({
                    'title': 'Không thể tải lên tài liệu',
                    'message': 'Thư mục không tồn tại.',
                    'buttons': {
                        'Đóng': {
                            'class': 'btn-confirm-yes',
                            'action': function () {
                            }
                        }
                    }
                });
            }
        },
        error: function (xhr) {
            CommonJS.alert(xhr.responseText);
        }
    });
}
function DownloadFileIndex(ID, gui) {
    $.ajax({
        type: "POST",
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/CheckkingFile',
        data: {
            ID: ID
        },
        cache: false,
        dataType: "json",
        success: function (data) {
            //alert(data);
            if (data == "Co") {
                location.href = "/ThuMucLuuTruArea/ThuMucLuuTru/DownloadFile?ID=" + ID;
            } else {
                $.confirm({
                    'title': 'Không tìm thấy tài liệu',
                    'message': 'Xin lỗi không thể tìm thấy tài liệu mà bạn đang yêu cầu.',
                    'buttons': {
                        'Đóng': {
                            'class': 'btn-warning',
                            'action': function () { } // Nothing to do in this case. You can as well omit the action property.
                        }
                    }
                });
            }
        }
    });
}
function FileDetail(ID, OPTION) {
    $.ajax({
        type: "POST",
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/FileDetail',
        data: {
            TAILIEU: ID,
            OPTION: OPTION
        },
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#CreateThuMuc").html(data);
            $("#CreateThuMuc").modal('show');
            $("#CreateThuMuc").find(".modal-dialog").css("width", "900px");
        }, error: function (err) {
            console.log(err);
        }
    });
}
$(document).on('click', '#THEMTAILIEU', function () {
    UploadSingle($(this).attr('data-pid'));
});