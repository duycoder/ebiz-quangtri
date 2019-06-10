function showFormLapKeHoach() {
    var checkcvcon = '@Model.LstTask.Count';
    if (checkcvcon == 0 || checkcvcon == "") {
        NotiError("Bạn phải tạo các công việc con trong kế hoạch");
    } else {
        $("#LAPKEHOACHCONGVIEC").modal('show');
    }
}
function removeUserPhoiHop(userid, taskid) {
    $.confirm({
        'title': 'Xác nhận xóa',
        'message': 'Bạn có chắc chắn muốn xóa người tham gia này?',
        'buttons': {
            'Đồng ý': {
                'class': 'btn-confirm-yes btn-primary',
                'action': function () {
                    $.ajax({
                        url: '/QuanLyCongViec/QuanLyCongViec/deleteNguoiPhoiHop',
                        type: 'post',
                        cache: false,
                        data: { userid: userid, taskid: taskid },
                        success: function (data) {
                            if (data.Type == "SUCCESS") {
                                $("#nguoiphoihop-" + userid).remove();
                                NotiSuccess(data.Message);
                            } else {
                                NotiError(data.Message);
                            }

                        },
                        error: function (err) {
                            CommonJS.alert(err.responseText);
                        }
                    });
                }
            },
            'Hủy bỏ': {
                'class': 'btn-danger',
                'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });
}
function deleteTaskFromPlan(id) {
    $.confirm({
        'title': 'Xác nhận xóa',
        'message': 'Bạn có chắc chắn muốn xóa công việc này?',
        'buttons': {
            'Đồng ý': {
                'class': 'btn-confirm-yes btn-primary',
                'action': function () {
                    $.ajax({
                        url: '/QuanLyCongViec/QuanLyCongViec/deleteTask',
                        type: 'post',
                        cache: false,
                        data: { id: id },
                        success: function (data) {
                            if (data.Type == "SUCCESS") {
                                $("#plan-subtask-" + id).remove();
                                NotiSuccess(data.Message);
                            } else {
                                NotiError(data.Message);
                            }

                        },
                        error: function (err) {
                            CommonJS.alert(err.responseText);
                        }
                    });
                }
            },
            'Hủy bỏ': {
                'class': 'btn-danger',
                'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });
}
function deleteTask(id) {
    $.confirm({
        'title': 'Xác nhận xóa',
        'message': 'Bạn có chắc chắn muốn xóa công việc này?',
        'buttons': {
            'Đồng ý': {
                'class': 'btn-confirm-yes btn-primary',
                'action': function () {
                    $.ajax({
                        url: '/QuanLyCongViec/QuanLyCongViec/deleteTask',
                        type: 'post',
                        cache: false,
                        data: { id: id },
                        success: function (data) {
                            if (data.Type == "SUCCESS") {
                                NotiSuccess(data.Message);
                                setTimeout(location.reload(), 2000);
                            } else {
                                NotiError(data.Message);
                            }

                        },
                        error: function (err) {
                            CommonJS.alert(err.responseText);
                        }
                    });
                }
            },
            'Hủy bỏ': {
                'class': 'btn-danger',
                'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });
}
function createFormSubTask(id) {
    var idx_tr = $("#" + id + " tr").last().data("index");
    idx_tr = parseInt(idx_tr) + 1;
    $.ajax({
        url: '/QuanLyCongViec/QuanLyCongViec/generateFormSubTask',
        type: 'post',
        cache: false,
        data: { idx: idx_tr },
        success: function (data) {
            $("#" + id).append(data);
        },
        error: function (err) {
            CommonJS.alert(err.responseText);
        }
    });
}
function XoaCongViec(idx) {
    $.confirm({
        'title': 'Xác nhận xóa',
        'message': 'Bạn có chắc chắn muốn xóa công việc đang nhập này?',
        'buttons': {
            'Đồng ý': {
                'class': 'btn-confirm-yes btn-primary',
                'action': function () {
                    $(".trsubtask_" + idx).remove();
                }
            },
            'Hủy bỏ': {
                'class': 'btn-danger',
                'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });

}
function submitDuyetKeHoach(id) {
    $.confirm({
        'title': 'Xác nhận xử lý',
        'message': 'Bạn có chắc chắn muốn duyệt kế hoạch?',
        'buttons': {
            'Đồng ý': {
                'class': 'btn-confirm-yes btn-primary',
                'action': function () {
                    $.ajax({
                        url: '/QuanLyCongViec/QuanLyCongViec/approveKeHoach',
                        type: 'post',
                        cache: false,
                        data: { id: id, status: $("#KETQUADUYETPLAN").val(), noidung: $("#DUYETKEHOACHCONGVIEC_NOIDUNG").val() },
                        success: function (data) {
                            NotiSuccess("Chuyển trạng thái thành công");
                            location.reload();
                        },
                        error: function (err) {
                            CommonJS.alert(err.responseText);
                        }
                    });
                }
            },
            'Hủy bỏ': {
                'class': 'btn-danger',
                'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });
}
function trinhKeHoach(id) {
    $.confirm({
        'title': 'Xác nhận xử lý',
        'message': 'Bạn có chắc chắn muốn trình kế hoạch?',
        'buttons': {
            'Đồng ý': {
                'class': 'btn-confirm-yes btn-primary',
                'action': function () {
                    $.ajax({
                        url: '/QuanLyCongViec/QuanLyCongViec/trinhKeHoach',
                        type: 'post',
                        cache: false,
                        data: { ID: id },
                        success: function (data) {
                            NotiSuccess("Chuyển trạng thái thành công");
                            location.reload();
                        },
                        error: function (err) {
                            CommonJS.alert(err.responseText);
                        }
                    });
                }
            },
            'Hủy bỏ': {
                'class': 'btn-danger',
                'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });
}
function beginProcess(id) {
    $.confirm({
        'title': 'Xác nhận xử lý',
        'message': 'Bạn có chắc chắn muốn bắt đầu thực hiện công việc này?',
        'buttons': {
            'Đồng ý': {
                'class': 'btn-confirm-yes btn-primary',
                'action': function () {
                    $.ajax({
                        url: '/QuanLyCongViec/QuanLyCongViec/BeginProcess',
                        type: 'post',
                        cache: false,
                        data: { ID: id },
                        success: function (data) {
                            NotiSuccess("Chuyển trạng thái thành công");
                            location.reload();
                        },
                        error: function (err) {
                            CommonJS.alert(err.responseText);
                        }
                    });
                }
            },
            'Hủy bỏ': {
                'class': 'btn-danger',
                'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });
}
function submitLapKeHoach() {
    if ($("#NGAYBATDAU_KEHOACH").val() == "") {
        $("#NGAYBATDAU_KEHOACH").focus();
        NotiError("Bạn phải nhập ngày dự kiến bắt đầu công việc");
        return false;
    }
    if ($("#NGAYKETTHUC_KEHOACH").val() == "") {
        $("#NGAYKETTHUC_KEHOACH").focus();
        NotiError("Bạn phải nhập ngày dự kiến kết thúc công việc");
        return false;
    }
    $("#LAPKEHOACHCONGVIECForm").submit();
}
function submitPhanHoiCongViec() {
    if ($("#PHANHOICONGVIEC").val() == "") {
        $("#PHANHOICONGVIEC").focus();
        NotiError("Bạn phải nhập nội dung phản hồi");
        return false;
    } else {
        $.confirm({
            'title': 'Xác nhận phản hồi',
            'message': 'Bạn có chắc chắn muốn thực hiện việc này?',
            'buttons': {
                'Đồng ý': {
                    'class': 'btn-confirm-yes btn-primary',
                    'action': function () {
                        $("#PhanHoiCongViecForm").submit();
                    }
                },
                'Hủy bỏ': {
                    'class': 'btn-danger',
                    'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
                }
            }
        });
    }

}
function rejectRequest(id, status) {
    $("#rejectId").val(id);
    $("#tuChoiGiaHanCongViec").modal("show");
}
function rejectExtendJob() {
    if ($("#tuchoi_lydo").val() == "") {
        NotiError("Bạn phải nhập lý do từ chối");
        $("#tuchoi_lydo").focus();
    } else {
        $.confirm({
            'title': 'Xác nhận từ chối gia hạn công việc',
            'message': 'Bạn có chắc chắn muốn từ chối duyệt gia hạn?',
            'buttons': {
                'Đồng ý': {
                    'class': 'btn-confirm-yes btn-primary',
                    'action': function () {
                        $.ajax({
                            url: '/QuanLyCongViec/QuanLyCongViec/ApproveExtendTask',
                            type: 'post',
                            cache: false,
                            data: { ID: $("#rejectId").val(), STATUS: 0, Mess: $("#tuchoi_lydo").val() },
                            success: function (data) {
                                NotiSuccess("Chuyển trạng thái thành công");
                                location.reload();
                            },
                            error: function (err) {
                                CommonJS.alert(err.responseText);
                            }
                        });
                    }
                },
                'Hủy bỏ': {
                    'class': 'btn-danger',
                    'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
                }
            }
        });
    }
}
function approveRequest(id, status, extenddate) {
    $("#approve_extend_hanhoanthanh").val(extenddate);
    $("#approve_butphe_hanhoanthanh").val(extenddate);
    $("#approveTaskId").val(id);
    $("#approveExtendTaskForm").modal("show");
}
function submitApproveExtendTask() {
    if ($("#approve_butphe_hanhoanthanh").val() == "") {
        NotiError("Bạn phải nhập ngày đồng ý gia hạn");
    } else {
        $.confirm({
            'title': 'Xác nhận phê duyệt gia hạn công việc',
            'message': 'Bạn có chắc chắn muốn phê duyệt?',
            'buttons': {
                'Đồng ý': {
                    'class': 'btn-confirm-yes btn-primary',
                    'action': function () {
                        $.ajax({
                            url: '/QuanLyCongViec/QuanLyCongViec/ApproveExtendTask',
                            type: 'post',
                            cache: false,
                            data: { ID: $("#approveTaskId").val(), STATUS: 1, Mess: $("#approve_phanhoi").val(), NgayExtend: $("#approve_butphe_hanhoanthanh").val() },
                            success: function (data) {
                                NotiSuccess("Chuyển trạng thái thành công");
                                location.reload();
                            },
                            error: function (err) {
                                CommonJS.alert(err.responseText);
                            }
                        });
                    }
                },
                'Hủy bỏ': {
                    'class': 'btn-danger',
                    'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
                }
            }
        });
    }
}

function submitExtendTask() {
    if ($("#EXTEND_NOIDUNG_COMMENT").val() == "") {
        $("#EXTEND_NOIDUNG_COMMENT").focus();
        NotiError("Bạn phải nhập lý do xin lùi hạn");
    } else {
        $("#ExtendTaskForm").submit();
    }
}
function saveTuDanhGia() {
    var checkRequired = RequireForm("TuDanhGiaCongViecForm");
    if (checkRequired == false) {
        NotiError("Kiểm tra lại các thông tin cần nhập");
    } else {

        if (parseInt($("#TDG_TUCHUCAO").val()) > 5 || parseInt($("#TDG_TRACHNHIEMLON").val()) > 5
            || parseInt($("#TDG_TUONGTACTOT").val()) > 5 || parseInt($("#TDG_TOCDONHANH").val()) > 5
            || parseInt($("#TDG_TIENBONHIEU").val()) > 5 || parseInt($("#TDG_THANHTICHVUOT").val()) > 5) {
            NotiError("Các giá trị phải là số và nhỏ hơn 5");
        } else {
            $("#TuDanhGiaCongViecForm").submit();
        }

    }
    return false;
}
function saveDuyetDanhGia() {
    var checkRequired = RequireForm("DiemDuyetDanhGiaCongViecForm");
    if (checkRequired == false) {
        NotiError("Kiểm tra lại các thông tin cần nhập");
    } else {

        if (parseInt($("#DD_TUCHUCAO").val()) > 5 || parseInt($("#DD_TRACHNHIEMLON").val()) > 5
            || parseInt($("#DD_TUONGTACTOT").val()) > 5 || parseInt($("#DD_TOCDONHANH").val()) > 5
            || parseInt($("#DD_TIENBONHIEU").val()) > 5 || parseInt($("#DD_THANHTICHVUOT").val()) > 5) {
            NotiError("Các giá trị phải là số và nhỏ hơn 5");
        } else {
            $.confirm({
                'title': 'Xác nhận duyệt đánh giá',
                'message': 'Bạn có chắc chắn muốn thực hiện việc này?',
                'buttons': {
                    'Đồng ý': {
                        'class': 'btn-confirm-yes btn-primary',
                        'action': function () {
                            $("#DiemDuyetDanhGiaCongViecForm").submit();
                        }
                    },
                    'Hủy bỏ': {
                        'class': 'btn-danger',
                        'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
                    }
                }
            });
            
        }

    }
    return false;
}
function extendTask() {
    $("#extendTask").modal("show");
}
function goBack() {
    window.history.back();
}
function assignJoinJob(id) {
    $.ajax({
        url: '/QuanLyCongViec/QuanLyCongViec/ShowMemberToJoinTask',
        type: 'post',
        cache: false,
        data: { TASKID: id },
        success: function (data) {
            $("#AssignTaskBody").html(data);
        },
        error: function (err) {
            CommonJS.alert(err.responseText);
        }
    });
    $("#AssignTaskDialog").modal("show");
}
function submitAssignTask() {
    if ($(".radio-xulychinh").length >= 1 && $(".radio-xulychinh input:radio:checked").length == 0) {
        NotiError("Bạn phải chọn người xử lý chính");
        return false;
    } else {
        $("#AssignTaskForm").submit();
    }
}
function assignTask(rootTaskId, subTaskId) {
    $.ajax({
        url: '/QuanLyCongViec/QuanLyCongViec/ShowMemberToAssignTask',
        type: 'post',
        cache: false,
        data: { TASKID: rootTaskId, SUBTASKID: subTaskId },
        success: function (data) {
            $("#AssignTaskBody").html(data);
        },
        error: function (err) {
            CommonJS.alert(err.responseText);
        }
    });
    $("#AssignTaskDialog").modal("show");
}
function showAllComment(id) {
    $(".message-reply-for-comment-" + id).show();
}
function saveCommentForTask(id, task_id) {
    if ($("#btn-input-for-task-" + id).val() == "") {
        $("#btn-input-for-task-" + id).focus();
        NotiError("Bạn phải nhập nội dung trao đổi");
    } else {
        $.ajax({
            url: '/QuanLyCongViec/QuanLyCongViec/SaveReplyForComment',
            type: 'post',
            cache: false,
            data: { COMMENT_ID: id, COMMENT: $("#btn-input-for-task-" + id).val(), TASK_ID: task_id },
            success: function (data) {
                NotiSuccess("Lưu nội dung trao đổi thành công");
                setTimeout(location.reload(), 2000);
            },
            error: function (err) {
                CommonJS.alert(err.responseText);
            }
        });
    }
}
function showFormComment(id) {
    $("#comment-for-task-" + id).show();
    $("#comment-for-task-" + id + " input").focus();
}
function saveNoiDungTraoDoi() {
    var checkRequired = RequireForm("SaveCommentRootLevelForm");
    if (checkRequired == false) {
        NotiError("Kiểm tra lại các thông tin cần nhập");
    } else {
        $("#SaveCommentRootLevelForm").submit();
    }
    return false;
}
function submitUpdateProgressTask(ID) {
    var checkRequired = RequireForm("UpdateProgressTaskForm");
    if (checkRequired == false) {
        NotiError("Kiểm tra lại các thông tin cần nhập");
    } else {
        $("#UpdateProgressTaskForm").submit();
    }
    return false;
}
function updateProgressTask() {
    $("#UpdateProgressTask").modal("show");
}
function finishTask(id) {
    $.confirm({
        'title': 'Xác nhận hoàn thành',
        'message': 'Bạn có chắc chắn đã hoàn thành công việc này?',
        'buttons': {
            'Đồng ý': {
                'class': 'btn-confirm-yes btn-primary',
                'action': function () {
                    $.ajax({
                        url: '/QuanLyCongViec/QuanLyCongViec/FinishSubTask',
                        type: 'post',
                        cache: false,
                        data: { ID: id },
                        success: function (data) {
                            NotiSuccess("Chuyển trạng thái thành công");
                            location.reload();
                        },
                        error: function (err) {
                            CommonJS.alert(err.responseText);
                        }
                    });
                }
            },
            'Hủy bỏ': {
                'class': 'btn-danger',
                'action': function () { }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });
}

function submitCreateSubTask() {
    var checkRequired = RequireForm("CreateSubTaskForm");
    if (checkRequired == false) {
        NotiError("Kiểm tra lại các thông tin cần nhập");
    } else {
        $("#TOTAL_CONGVIEC").val($("#body_subtask_id tr").last().data("index"));
        $("#CreateSubTaskForm").submit();
    }
    return false;
}
function createSubTask() {
    $("#CreateSubTask").modal("show");
}
function RequireForm(formID) {
    var check_err = true;
    var item = $("#" + formID + " .required");
    item.each(function () {
        //var parent = $(this).parents(" .form-group").first();
        var errText = $(this).next().find(".error");
        if ($(this).val() == null || $(this).val().length == 0) {
            console.log($(this));
            errText.html("Bạn phải nhập thông tin này");
            errText.css('display', 'inline');
            check_err = false;
        } else {
            errText.css('display', 'none');
        }
    })

    return check_err;
}

function searchMainProcess(event) {
    var value = $(event.currentTarget).val().trim();
    value = CommonJS.removeVietnameseChars(value);

    if ($('input[type=radio][name=change-giaoviec]:checked').val() == '2') {
        if (value !== '') {
            $.each($('#box-nguoixulychinh .hoten-nguoixulychinh'), function (index, item) {
                var name = $(this).text();
                name = CommonJS.removeVietnameseChars(name);
                if (name.indexOf(value) === -1) {
                    $(this).parent().hide();
                    if ($(this).parent().parent().find(".radio-xulychinh:visible").length == 0) {
                        $(this).parent().parent().parent().hide();
                    }
                } else {
                    $(this).parent().parent().parent().show();
                    $(this).parent().show();
                }
            });
        } else {
            $('#box-nguoixulychinh .radio-xulychinh').show();
            $("#box-nguoixulychinh .panel-info-main-process").show();
        }
    } else {
        if (value !== '') {
            $.each($('#box-nguoixulychinh .panel-root .hoten-nguoixulychinh'), function (index, item) {
                var name = $(this).text();
                name = CommonJS.removeVietnameseChars(name);
                if (name.indexOf(value) === -1) {
                    $(this).parent().hide();
                    if ($(this).parent().parent().find(".radio-xulychinh:visible").length == 0) {
                        $(this).parent().parent().parent().hide();
                    }
                } else {
                    $(this).parent().parent().parent().show();
                    $(this).parent().show();
                }
            });
        } else {
            $('#box-nguoixulychinh .panel-root .radio-xulychinh').show();
            $("#box-nguoixulychinh .panel-root .panel-info-main-process").show();
        }
    }
}
function searchSupportProcess(event) {
    var value = $(event.currentTarget).val().trim();
    value = CommonJS.removeVietnameseChars(value);
    if ($('input[type=radio][name=change-giaoviec]:checked').val() == '2') {
        if (value !== '') {
            $.each($('#box-nguoithamgia .hoten-nguoithamgia'), function (index, item) {
                var name = $(this).text();
                name = CommonJS.removeVietnameseChars(name);
                if (name.indexOf(value) === -1) {
                    $(this).parent().hide();
                    if ($(this).parent().parent().find(".radio-thamgia:visible").length == 0) {
                        $(this).parent().parent().parent().hide();
                    }
                } else {
                    $(this).parent().parent().parent().show();
                    $(this).parent().show();
                }
            });
        } else {
            $('#box-nguoithamgia .radio-thamgia').show();
            $("#box-nguoithamgia .panel-info-support-process").show();
        }
    } else {
        if (value !== '') {
            $.each($('#box-nguoithamgia .panel-root .hoten-nguoithamgia'), function (index, item) {
                var name = $(this).text();
                name = CommonJS.removeVietnameseChars(name);
                if (name.indexOf(value) === -1) {
                    $(this).parent().hide();
                    if ($(this).parent().parent().find(".radio-thamgia:visible").length == 0) {
                        $(this).parent().parent().parent().hide();
                    }
                } else {
                    $(this).parent().parent().parent().show();
                    $(this).parent().show();
                }
            });
        } else {
            $('#box-nguoithamgia .panel-root .radio-thamgia').show();
            $("#box-nguoithamgia .panel-root .panel-info-support-process").show();
        }
    }


}
function showAllSubTask(id) {
    $.ajax({
        url: '/QuanLyCongViec/QuanLyCongViec/ShowAllTask',
        type: 'post',
        cache: false,
        data: { id: id },
        success: function (data) {
            $("#ShowAllTaskBody").html(data);
        },
        error: function (err) {
            CommonJS.alert(err.responseText);
        }
    });
    $("#ShowAllTaskDialog").modal("show");
}

var pageFunction = function () {
    $(".selectpicker1").selectpicker();
    $("#LAPKEHOACHCONGVIEC .datepicker1").datepicker({
        dateFormat: 'dd/mm/yy',
        prevText: '<i class="fa fa-chevron-left"></i>',
        nextText: '<i class="fa fa-chevron-right"></i>',
        changeMonth: true,
        changeYear: true,
        yearRange: "0:+2"
    });
    CKEDITOR.replace('CACBUOCTHUCHIEN', {
        height: "100px",
        toolbarGroups: [
            { "name": "basicstyles", "groups": ["basicstyles"] },
            { "name": "links", "groups": ["links"] },
            { "name": "paragraph", "groups": ["list", "blocks"] },
            { "name": "document", "groups": ["mode"] },
            { "name": "insert", "groups": ["insert"] },
            { "name": "styles", "groups": ["styles"] },
            { "name": "about", "groups": ["about"] }
        ],
        removeButtons: 'Underline,Strike,Subscript,Superscript,Anchor,Styles,Specialchar'
    });
    CKEDITOR.replace('MUCTIEU_CONGVIEC', {
        height: "100px",
        toolbarGroups: [
            { "name": "basicstyles", "groups": ["basicstyles"] },
            { "name": "links", "groups": ["links"] },
            { "name": "paragraph", "groups": ["list", "blocks"] },
            { "name": "document", "groups": ["mode"] },
            { "name": "insert", "groups": ["insert"] },
            { "name": "styles", "groups": ["styles"] },
            { "name": "about", "groups": ["about"] }
        ],
        removeButtons: 'Underline,Strike,Subscript,Superscript,Anchor,Styles,Specialchar',
    });
}

$(document).ready(function () {
    loadScript("/ckeditor/ckeditor.js", function () {
        loadScript("/Content/select/js/bootstrap-select.js", pageFunction);
    });
})
$(document).on("keypress", ".comment", function (evt) {
    if (evt.which === 13) {
        $(this).next().find(".btn-primary").click();
    }
})
function TuDanhGiaCongViec() {
    $.confirm({
        'title': 'Xác nhận đánh giá công việc',
        'message': 'Bạn muốn đánh giá theo tiêu chí nào',
        'buttons': {
            'Sử dụng tiêu chí 6T': {
                'class': 'btn-confirm-yes btn-primary',
                'action': function () {
                    $("#TuDanhGiaCongViec").modal("show");
                }
            },
            'Đánh giá theo thang điểm chọn': {
                'class': 'btn-success',
                'action': function () {
                    $("#danhGiaTheoThangDiem").modal("show");
                }	// Nothing to do in this case. You can as well omit the action property.
            },
            'Đóng': {
                'class': 'btn-danger',
                'action': function () {
                }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });
}
function saveDuyetDanhGiaTheoThangDiem() {
    $.confirm({
        'title': 'Xác nhận đánh giá công việc',
        'message': 'Bạn chắc chắn muốn gửi kết quả này',
        'buttons': {
            'Đồng ý': {
                'class': 'btn-confirm-yes btn-primary',
                'action': function () {                    
                    $("#danhGiaTheoThangDiem").modal("hide");
                    $("#danhGiaTheoThangDiemForm").submit();
                }
            },
            'Hủy': {
                'class': 'btn-success',
                'action': function () {
                }	// Nothing to do in this case. You can as well omit the action property.
            }
        }
    });
}