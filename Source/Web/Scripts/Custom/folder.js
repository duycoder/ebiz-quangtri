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
    //$("#browser li").each(function () {
    //    if ($(this).attr("data-fid") == "F_" + ParentID) {
    //        this1 = $(this);
    //        loadDataById($(this), true);
    //        return;
    //    }
    //});

    //if ($("#folder_" + ParentID).length > 0) {
    //    $("#folder_" + ParentID).click();
    //} else {
    $("#Path li:last-child span.folder").click();
    //}
}
function GetPath(pid) {
    $.ajax({
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/GetUrlBar',
        type: 'GET',
        data: {
            pID: pid
        },
        dataType: "json",
        success: function (d) {
            $("#URLPath").html("");
            var $ul = $("<ul id='Path' ></ul>");
            $ul.append($("<li data-fid='F_0'></li>").append(
                "<span class='folder_root' data-loaded='false' data-pid='0'><i class=\"fa fa-folder-open-o url-folder\" style='margin-right:3px'></i>Gốc<i class=\"fa fa-chevron-right\"></i></span>"
            )
            )
            if (d.length > 1) {
                for (var i = d.length - 1; i >= 0; i--) {
                    if (i == 0) {
                        $ul.append(
                            $("<li data-fid='F_" + d[i].ID + "'></li>").append(
                                "<span class='folder' data-loaded='false' data-pid='" + d[i].ID + "'><i class=\"fa fa-folder-open-o url-folder\" style='margin-right:3px'></i>" + d[i].TENTHUMUC + "</span>"
                            )
                        )
                    } else {
                        $ul.append(
                            $("<li data-fid='F_" + d[i].ID + "'></li>").append(
                                "<span class='folder' data-loaded='false' data-pid='" + d[i].ID + "'><i class=\"fa fa-folder-open-o url-folder\" style='margin-right:3px'></i>" + d[i].TENTHUMUC + "<i class=\"fa fa-chevron-right\"></i></span>"
                            )
                        )
                    }
                }
            } else if (d[0] != null) {
                $ul.append(
                    $("<li data-fid='F_" + d[0].ID + "'></li>").append(
                        "<i class=\"fa fa-folder-open-o url-folder\" style='margin-right:3px'></i><span class='folder' data-loaded='false' data-pid='" + d[0].ID + "'>" + d[0].TENTHUMUC + "</span>"
                    )
                )
            } else {
                $ul.append("");
            }
            $("#URLPath").append($ul);
        },
        error: function (xhr) {
            CommonJS.alert(xhr.responseText);
        }
    });
}
$(document).on('click', '.folder,.folder_root,.folder-left,.folder-right,.folder-both,.folder-write,.folder-both,.folder-read', function () {
    var this1 = $(this);
    this1.closest("li").find("ul").remove("ul");
    var isLoaded = $(this).attr('data-loaded');
    var id = $(this1).attr('data-pid');
    $("#SEARCH_FOLDER_ID").val(id);
    if (id == 0) {
        currentFolderId = 0;
        $(".hinet-table tfoot").remove();
        reloadGrid();
        reloadTable();
        $("#URLPath").html("");
        GetPath(0);
        $("#THEMTHUMUC").attr("data-pid", "0");
        $("#THEMTAILIEU").hide();
    } else {
        this1.attr("data-loaded", true);
        if ($(this1).closest("li").find(".folder-item").length > 0) {
            //if ($("#F_" + id).find(".folder_arrow").length > 0) {
            //    $("#F_" + id).find(".folder_arrow").click();
            //} else {
            loadDataById(this1, false);
            //}
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
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/GetChild',
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
            if (ids.indexOf(pid) >= 0) {
                currentFolderId = pid;
            }
            if (pid == $("#THUMUC_CONGVIEC").val() || pid == $("#THUMUC_VANBAN").val() || pid == $("#THUMUC_VANBANDEN").val()) {
                $("#THEMTAILIEU").hide();
                $("#THEMTHUMUC").hide();
            } else if (ids.indexOf(pid) >= 0) {
                $("#THEMTAILIEU").hide();
                $("#THEMTHUMUC").show();
            } else {
                $("#THEMTAILIEU").show();
                $("#THEMTHUMUC").show();
            }
            if (ids.indexOf(currentFolderId) > 0) {
                switch (ids.indexOf(currentFolderId)) {
                    case 1:
                        //Phong ban
                        if ($("#ACCESS_FOLDER").val() == $("#ACCESS_PRIVATE").val()) {
                            $("#THEMTAILIEU").hide();
                            $("#THEMTHUMUC").hide();
                        }
                        break;
                    case 2:
                        //Don vi
                        if ($("#ACCESS_FOLDER").val() == $("#ACCESS_PRIVATE").val()
                            || $("#ACCESS_FOLDER").val() == $("#ACCESS_DEPT").val()) {
                            $("#THEMTAILIEU").hide();
                            $("#THEMTHUMUC").hide();
                        }
                        break;
                    case 3:
                        //Tap doan
                        if ($("#ACCESS_FOLDER").val() != $("#ACCESS_SYSTEM").val()) {
                            $("#THEMTAILIEU").hide();
                            $("#THEMTHUMUC").hide();
                        }
                        break;
                }
            } else if (ids.indexOf(currentFolderId) == 0) {
                //$("#THEMTAILIEU").hide();
                $("#THEMTHUMUC").show();
            }
            GetPath($(this1).attr('data-pid'));
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
                    if (ele.USER_ID == parseInt($("#USER_ID").val())) {
                        ele.PERMISSION = parseInt($("#CAN_READ_WRITE").val())
                    }
                    if (ele.IS_THUMUC) {
                        var thumuc_trangthai = "";
                        if (ele.CLASS != null && ele.CLASS != "") {
                            if (ele.PERMISSION == parseInt($("#CAN_WRITE").val())) {
                                thumuc_trangthai += "<span class='folder-write folder-left-padding " + ele.CLASS + "' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";
                            } else if (ele.PERMISSION == parseInt($("#CAN_READ").val())) {
                                thumuc_trangthai += "<span class='folder-write folder-left-padding " + ele.CLASS + "' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";
                            } else if (ele.USER_ID != parseInt($("#USER_ID").val())) {
                                thumuc_trangthai += "<span class='folder-both folder-left-padding " + ele.CLASS + "' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";
                            } else {
                                thumuc_trangthai += "<span class='folder folder-left-padding " + ele.CLASS + "' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";
                            }
                        } else {
                            if (ele.PERMISSION == parseInt($("#CAN_WRITE").val())) {
                                thumuc_trangthai += "<span class='folder-write allsystem1' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";
                            } else if (ele.PERMISSION == parseInt($("#CAN_READ").val())) {
                                thumuc_trangthai += "<span class='folder allsystem1' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";
                            } else if (ele.USER_ID != parseInt($("#USER_ID").val())) {
                                thumuc_trangthai += "<span class='folder-both allsystem1' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";
                            } else {
                                thumuc_trangthai += "<span class='folder allsystem1' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";
                            }
                        }
                        $ul.append(
                            $("<li data-fid='F_" + ele.ID + "' class='collapsed' id='F_" + ele.ID + "'></li>").append(
                                thumuc_trangthai
                            )
                        )
                    }
                    var ngaytao = ToDate(ele.NGAYTAO);
                    if (ele.IS_THUMUC) {
                        var thumuc_trangthai = "";
                        var share = "";
                        thumuc += "<tr  class='folder-item'>";
                        thumuc += "<td></td>";
                        if (ele.CLASS != null && ele.CLASS != "") {
                            if (ele.PERMISSION == parseInt($("#CAN_WRITE").val())) {
                                thumuc_trangthai = "<span data-per='" + ele.PERMISSION + "' data-sort='0' class='folder-write folder-common " + ele.CLASS + "' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";;
                            } else if (ele.PERMISSION == parseInt($("#CAN_READ").val())) {
                                thumuc_trangthai = "<span data-per='" + ele.PERMISSION + "' data-sort='0' class='folder-read folder-common " + ele.CLASS + "' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";;
                            } else if (ele.USER_ID != parseInt($("#USER_ID").val())) {
                                thumuc_trangthai = "<span data-per='" + ele.PERMISSION + "' data-sort='0' class='folder-both folder-common " + ele.CLASS + "' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";;
                            } else {
                                thumuc_trangthai = "<span data-per='" + ele.PERMISSION + "' data-sort='0' class='folder folder-common " + ele.CLASS + "' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";;
                            }
                        } else {
                            if (ele.PERMISSION == parseInt($("#CAN_WRITE").val())) {
                                thumuc_trangthai = "<span data-per='" + ele.PERMISSION + "' data-sort='0' class='folder-write folder-common allsystem' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";;
                            } else if (ele.PERMISSION == parseInt($("#CAN_READ").val())) {
                                thumuc_trangthai = "<span data-per='" + ele.PERMISSION + "' data-sort='0' class='folder-read folder-common allsystem' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";;
                            } else if (ele.USER_ID != parseInt($("#USER_ID").val())) {
                                thumuc_trangthai = "<span data-per='" + ele.PERMISSION + "' data-sort='0' class='folder-both folder-common allsystem' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";;
                            } else {
                                thumuc_trangthai = "<span data-per='" + ele.PERMISSION + "' data-sort='0' class='folder folder-common allsystem' title ='" + ele.TENTHUMUC + "' data-loaded='false' data-pid='" + ele.ID + "'>" + ele.TENTHUMUC + "</span>";;
                            }
                        }
                        thumuc += "<td data-fid='F_" + ele.ID + "'><div style='width:100%;float:left;margin-top:-6px !important' class='TENTHUMUC'>" + thumuc_trangthai + "</div></td>";
                        thumuc += "<td>" + ele.TEN_DONVI + "</td>";
                        thumuc += "<td>" + ele.TEN_NGUOITAO + "</td>";
                        thumuc += "<td>" + ngaytao + "</td>";
                        thumuc += "<td>" + initContext(ele) + "</td>";
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
                        if (ele.PERMISSION == parseInt($("#CAN_WRITE").val())) {
                            tailieu += "<td data-fid='F_" + ele.ID + "'><span data-per='" + ele.PERMISSION + "' data-sort='0' class='" + extension.split('/')[1] + " " + docx + " " + extension.split('.')[1] + " file-write' data-file='true' data-pid='" + ele.ID + "'>" + htmlDecode(ele.TENTHUMUC) + "</span></td>";
                        } else if (ele.PERMISSION == parseInt($("#CAN_READ").val())) {
                            tailieu += "<td data-fid='F_" + ele.ID + "'><span data-per='" + ele.PERMISSION + "' data-sort='0' class='" + extension.split('/')[1] + " " + docx + " " + extension.split('.')[1] + " file-read' data-file='true' data-pid='" + ele.ID + "'>" + htmlDecode(ele.TENTHUMUC) + "</span></td>";
                        } else {
                            if (ids.indexOf(currentFolderId) == 3 && ele.USER_ID != parseInt($("#USER_ID").val())) {
                                tailieu += "<td data-fid='F_" + ele.ID + "'><span data-per='" + ele.PERMISSION + "' data-sort='0' class='" + extension.split('/')[1] + " " + docx + " " + extension.split('.')[1] + " file-both' data-file='true' data-pid='" + ele.ID + "'>" + htmlDecode(ele.TENTHUMUC) + "</span></td>";
                            } else {
                                tailieu += "<td data-fid='F_" + ele.ID + "'><span data-per='" + ele.PERMISSION + "' data-sort='0' class='" + extension.split('/')[1] + " " + docx + " " + extension.split('.')[1] + " public-files' data-file='true' data-pid='" + ele.ID + "'>" + htmlDecode(ele.TENTHUMUC) + "</span></td>";
                            }
                        }
                        tailieu += "<td>" + ele.TEN_DONVI + "</td>";
                        tailieu += "<td>" + ele.TEN_NGUOITAO + "</td>";
                        tailieu += "<td>" + ngaytao + "</td>";
                        tailieu += "<td>" + initContext(ele) + "</td>";
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
function GetFullPath(pid) {
    $.ajax({
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/GetUrlBar',
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
$(document).on('click', '#THEMTAILIEU', function () {
    UploadSingle($(this).attr('data-pid'));
});
function htmlDecode(html) {
    return html.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}
function initContext(data) {
    var id = data.ID;
    var isFolder = data.IS_THUMUC;
    var html = '<div class="dropdown">';
    html += '<button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
    html += 'Thao tác';
    html += '</button>';
    html += '<div class="dropdown-menu" aria-labelledby="dropdownMenuButton">';
    if (data.USER_ID == parseInt($("#USER_ID").val())) {
        data.PERMISSION = parseInt($("#CAN_READ_WRITE").val())
    }
    if (isFolder) {
        if (data.PERMISSION == parseInt($("#CAN_WRITE").val())) {
            html += '<a class="dropdown-item" onclick="UploadSingle(' + id + ')" href="javascript:void(0)"><i class="fa fa-cloud-upload"></i> Upload 1 file</a>';
            html += '<a class="dropdown-item" onclick="UploadFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-upload"></i> Upload nhiều file (bằng cách chọn từng file)</a>';
            html += '<a class="dropdown-item" onclick="UploadZipFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-file-archive-o"></i> Upload file Zip/Rar và giải nén</a>';
            html += '<a class="dropdown-item" onclick="DragDrop(' + id + ')" href="javascript:void(0)"><i class="fa fa-icon-dropbox"></i> Upload nhiều file (bằng cách chọn nhiều)</a>';
            html += '<a class="dropdown-item" onclick="CreateThuMuc(' + id + ',false)" href="javascript:void(0)"><i class="fa fa-folder-open-o"></i> Thêm thư mục</a>';
            //html += '<a class="dropdown-item" onclick="CreateThuMuc(' + id + ',true)" href="javascript:void(0)"><i class="fa fa-edit"></i> Cập nhật</a>';
        } else if (data.PERMISSION == parseInt($("#CAN_READ").val())) {
            html += '<a class="dropdown-item" href="/ThuMucLuuTruArea/ThuMucLuuTru/DownloadZipFile?id=' + id + '"><i class="fa fa-download"></i> Download dạng ZIP</a>';
        } else if (data.USER_ID != parseInt($("#USER_ID").val())) {
            html += '<a class="dropdown-item" href="/ThuMucLuuTruArea/ThuMucLuuTru/DownloadZipFile?id=' + id + '"><i class="fa fa-download"></i> Download dạng ZIP</a>';
            html += '<a class="dropdown-item" onclick="UploadSingle(' + id + ')" href="javascript:void(0)"><i class="fa fa-cloud-upload"></i> Upload 1 file</a>';
            html += '<a class="dropdown-item" onclick="UploadFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-upload"></i> Upload nhiều file (bằng cách chọn từng file)</a>';
            html += '<a class="dropdown-item" onclick="UploadZipFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-file-archive-o"></i> Upload file Zip/Rar và giải nén</a>';
            html += '<a class="dropdown-item" onclick="DragDrop(' + id + ')" href="javascript:void(0)"><i class="fa fa-icon-dropbox"></i> Upload nhiều file (bằng cách chọn nhiều)</a>';
            html += '<a class="dropdown-item" onclick="CreateThuMuc(' + id + ',false)" href="javascript:void(0)"><i class="fa fa-folder-open-o"></i> Thêm thư mục</a>';
            html += '<a class="dropdown-item" onclick="SharingFolder(' + id + ',true)" href="javascript:void(0)"><i class="fa fa-share-alt"></i> Chia sẻ</a>';
        } else {
            html += '<a class="dropdown-item" href="/ThuMucLuuTruArea/ThuMucLuuTru/DownloadZipFile?id=' + id + '"><i class="fa fa-download"></i> Download dạng ZIP</a>';
            html += '<a class="dropdown-item" onclick="UploadSingle(' + id + ')" href="javascript:void(0)"><i class="fa fa-cloud-upload"></i> Upload 1 file</a>';
            html += '<a class="dropdown-item" onclick="UploadFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-upload"></i> Upload nhiều file (bằng cách chọn từng file)</a>';
            html += '<a class="dropdown-item" onclick="UploadZipFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-file-archive-o"></i> Upload file Zip/Rar và giải nén</a>';
            html += '<a class="dropdown-item" onclick="DragDrop(' + id + ')" href="javascript:void(0)"><i class="fa fa-icon-dropbox"></i> Upload nhiều file (bằng cách chọn nhiều)</a>';
            html += '<a class="dropdown-item" onclick="CreateThuMuc(' + id + ',false)" href="javascript:void(0)"><i class="fa fa-folder-open-o"></i> Thêm thư mục</a>';
            html += '<a class="dropdown-item" onclick="CreateThuMuc(' + id + ',true)" href="javascript:void(0)"><i class="fa fa-edit"></i> Cập nhật</a>';
            html += '<a class="dropdown-item" onclick="SharingFolder(' + id + ',true)" href="javascript:void(0)"><i class="fa fa-share-alt"></i> Chia sẻ</a>';
            html += '<a class="dropdown-item" onclick="CheckFolderRemove(' + id + ')" href="javascript:void(0)"><i class="glyphicon glyphicon-trash"></i> Xóa</a>';

        }
    } else {
        if (data.PERMISSION == parseInt($("#CAN_READ").val())) {
            html += '<a class="dropdown-item" onclick="DownloadFileIndex(' + id + ',null)" href="javascript:void(0)"><i class="fa fa-download"></i> Tải xuống</a>';
        } else if (data.PERMISSION == parseInt($("#CAN_WRITE").val())) {
            //html += '<a class="dropdown-item" onclick="DoiTenFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-edit"></i> Đổi tên</a>';
            html += '<a class="dropdown-item" onclick="ViewDetail(' + id + ')" href="javascript:void(0)"><i class="fa fa-eye"></i> Chi tiết</a>';
            //html += '<a class="dropdown-item" onclick="EditFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-pencil-square-o"></i> Chỉnh sửa</a>';
        } else if (data.USER_ID != parseInt($("#USER_ID").val())) {
            html += '<a class="dropdown-item" onclick="DownloadFileIndex(' + id + ',null)" href="javascript:void(0)"><i class="fa fa-download"></i> Tải xuống</a>';
            //html += '<a class="dropdown-item" onclick="SharingFile(' + id + ')" href="javascript:void(0)"><i class="fa-share-alt"></i> Chia sẻ</a>';
            html += '<a class="dropdown-item" onclick="ViewDetail(' + id + ')" href="javascript:void(0)"><i class="fa fa-eye"></i> Chi tiết</a>';
            html += '<a class="dropdown-item" onclick="ManagerVersion(' + id + ')" href="javascript:void(0)"><i class="fa fa-archive"></i> Quản lý phiên bản</a>';
            //html += '<a class="dropdown-item" onclick="CheckFileRemove(' + id + ')" href="javascript:void(0)"><i class="glyphicon glyphicon-trash"></i> Xóa</a>';
        } else {
            html += '<a class="dropdown-item" onclick="DownloadFileIndex(' + id + ',null)" href="javascript:void(0)"><i class="fa fa-download"></i> Tải xuống</a>';
            html += '<a class="dropdown-item" onclick="DoiTenFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-edit"></i> Đổi tên</a>';
            html += '<a class="dropdown-item" onclick="SharingFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-share-alt"></i> Chia sẻ</a>';
            html += '<a class="dropdown-item" onclick="ViewDetail(' + id + ')" href="javascript:void(0)"><i class="fa fa-eye"></i> Chi tiết</a>';
            html += '<a class="dropdown-item" onclick="EditFile(' + id + ')" href="javascript:void(0)"><i class="fa fa-pencil-square-o"></i> Chỉnh sửa</a>';
            html += '<a class="dropdown-item" onclick="ManagerVersion(' + id + ')" href="javascript:void(0)"><i class="fa fa-archive"></i> Quản lý phiên bản</a>';
            html += '<a class="dropdown-item" onclick="CheckFileRemove(' + id + ')" href="javascript:void(0)"><i class="glyphicon glyphicon-trash"></i> Xóa</a>';
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
function CheckFolderRemove(THUMUC_ID) {
    $.ajax({
        type: "POST",
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/KiemTraXoaThuMuc',
        data: {
            THUMUC_ID: THUMUC_ID
        },
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data == "Co") {
                RemoveThuMuc(THUMUC_ID);
            } else {
                $.confirm({
                    'title': 'Không thể xóa thư mục này!',
                    'message': 'Bạn không có quyền xóa thư mục này.',
                    'buttons': {
                        'Đóng': {
                            'class': 'btn-info',
                            'action': function () { } // Nothing to do in this case. You can as well omit the action property.
                        }
                    }
                });
            }
        }
    });
}
function RemoveThuMuc(ID) {
    $.confirm({
        'title': 'Xác nhận xóa',
        'message': 'Bạn có chắc chắn muốn xóa thư mục này?',
        'buttons': {
            'Xóa': {
                'class': 'btn-confirm-yes',
                'action': function () {
                    $.ajax({
                        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/Delete',
                        type: 'post',
                        cache: false,
                        data: { ID: ID },
                        success: function (data) {
                            NotiSuccess(data.Message);
                            if (ID == 0 || $("#THEMTHUMUC").attr("data-pid") <= 0) {
                                reloadGrid();
                                reloadTable();
                            } else {
                                var folderSize = $("#F_" + ID).closest("li").find("ul li").length;
                                var parentId = $("#F_" + ID).parent().closest("li").find(".folder").attr("data-pid");
                                if (folderSize > 0) {
                                    $("#folder_" + parentId).click();
                                    $("#folder_" + parentId).click();
                                }
                                if ($("#F_" + parentId).find("ul").html() == undefined) {
                                    $("#F_" + parentId).find(".folder_arrow").remove();
                                }
                                $("#treeview li").each(function () {
                                    if ($(this).attr("data-fid") == "F_" + ID) {
                                        $(this).remove();
                                    }
                                });
                                $(".hinet-table tbody td").each(function () {
                                    if ($(this).attr("data-fid") == "F_" + ID) {
                                        $(this).closest("tr").remove();
                                    }
                                });

                            }
                        },
                        error: function (err) {
                            CommonJS.alert(err.responseText);
                        }, complete: function () {
                            reloadIndex();
                        }
                    });
                }
            },
            'Không xóa': {
                'class': 'btn-info',
                'action': function () { }
            }
        }
    });
}
function DoiTenFile(ID) {
    $.ajax({
        type: "POST",
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/RenameFile',
        data: {
            ID: ID
        },
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#CreateThuMuc").html(data);
            $("#CreateThuMuc").modal('show');
        }
    });
}
function RenameThuMuc(ID) {
    $.ajax({
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/RenameThuMuc',
        type: 'post',
        cache: false,
        data: {
            ID: ID
        },
        success: function (data) {
            if (data.trim().length > 0) {
                $("#CreateThuMuc").html(data);
                $("#CreateThuMuc").modal("show");
            } else {
                $.confirm({
                    'title': 'Không thể cập nhập!',
                    'message': 'Thư mục này đang được trình cục trưởng bạn không thể cập nhập vào lúc này!',
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
function CheckFileRemove(TAILIEU) {
    $.ajax({
        type: "POST",
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/KiemTraXoaTaiLieu',
        data: {
            TAILIEU: TAILIEU
        },
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data == "Co") {
                XoaFile(TAILIEU);
            } else {
                $.confirm({
                    'title': 'Không thể xóa tài liệu này!',
                    'message': 'Bạn không có quyền xóa tài liệu này.',
                    'buttons': {
                        'Đóng': {
                            'class': 'btn-info',
                            'action': function () { } // Nothing to do in this case. You can as well omit the action property.
                        }
                    }
                });
            }
        }
    });
}
function XoaFile(ID) {
    $.confirm({
        'title': 'Xác nhận xóa',
        'message': 'Bạn có chắc chắn muốn xóa tài liệu này?Dữ liệu sẽ không thể phục hồi',
        'buttons': {
            'Xóa': {
                'class': 'btn-confirm-yes',
                'action': function () {
                    $.ajax({
                        type: "POST",
                        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/XoaTaiLieu',
                        data: {
                            TAILIEU: ID
                        },
                        cache: false,
                        dataType: "json",
                        success: function (data) {
                            $(".hinet-table tfoot td").each(function () {
                                if ($(this).attr("data-fid") == "F_" + ID) {
                                    $(this).closest("tr").remove();
                                }
                            });
                            NotiSuccess(data);
                        }, complete: function () {
                            reloadIndex();
                        }
                    });
                }
            },
            'Không xóa': {
                'class': 'btn-info',
                'action': function () { } // Nothing to do in this case. You can as well omit the action property.
            }
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
function ManagerVersion(ID) {
    $.ajax({
        type: "POST",
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/VersionFile',
        data: {
            TAILIEU: ID
        },
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#CreateThuMuc").html(data);
            $("#CreateThuMuc").modal('show');
            $("#CreateThuMuc").find(".modal-dialog").css("width", "950px");
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
            //if (OPTION == "chitiet") {
            //    $("#CreateThuMuc").find(".modal-dialog").css("width", "900px");
            //} else {
            $("#CreateThuMuc").find(".modal-dialog").css("width", "900px");
            //}
            //var curHeight = $("#file-detail-body").height();
            //if (curHeight >= 365) {
            //    $("#file-detail-body").css("height", "450px");
            //}
        }, error: function (err) {
            console.log(err);
        }
    });
}
function SharingFolder(id) {
    $.ajax({
        type: "POST",
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/Sharing',
        data: {
            id: id,
            isFolder: true
        },
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#CreateThuMuc").html(data);
            $("#CreateThuMuc").modal('show');
        }
    });
}
function SharingFile(id) {
    $.ajax({
        type: "POST",
        url: '/THUMUCLUUTRUArea/THUMUCLUUTRU/Sharing',
        data: {
            id: id,
            isFolder: false
        },
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#CreateThuMuc").html(data);
            $("#CreateThuMuc").modal('show');
        }
    });
}