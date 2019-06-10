
function showWfStepChangeView(id, startStatus, endStatus) {
    $.ajax({
        url: '/HSVanBanDiArea/HSVanBanDi/ShowWfStepChangeView',
        type: 'post',
        dataType: 'html',
        data: {
            id: id,
            startStatus: startStatus,
            endStatus: endStatus
        },
        success: function (result) {
            if (result !== null) {
                $('#modal--wf-stepchange .modal-content').html(result);

                $('#modal--wf-stepchange').modal('show');
            } else {
                alert('Có lỗi khi tải dữ liệu');
            }
        }
    });
}

function showAdditionalUser(isMainProcess) {
    var listRestrictedUsersIds = [];
    $.each($('#modal--wf-stepchange .USER_PROCESS'), function (index, item) {
        listRestrictedUsersIds.push(item.value);
    });
    $.ajax({
        url: '@Url.Action("ShowAdditionalUser")',
        type: 'post',
        contentType: 'application/json;charset=utf-8',
        data: JSON.stringify({
            listRestrictedUsersIds: listRestrictedUsersIds,
            isMainProcess: isMainProcess
        }),
        dataType: 'html',
        success: function (result) {
            $('#modal--wf-additional-recipient .modal-body').html(result);
            $('#modal--wf-additional-recipient').modal('show');
        }, error: function (err) {
            CommonJS.showNotifyError('Tải dữ liệu người dùng không thành công');
        }
    });
}

function addNewRecipient() {
    var htmlAdditionalProcessUsers = '';
    var isMainProcess = $('#modal--wf-additional-recipient #IS_MAIN_PROCESS').val() == "1" ? true : false;
    $('#block--add-recipients').find('input[type=checkbox]:checked').each(function (index, item) {
        var userId = $(this).val();
        var userName = $(this).next('i').next('.USER_ADD_PROCESS_NAME').text();
        if (index == 0) {
            htmlAdditionalProcessUsers += '<tr class="clearfix row-addtional">';
            htmlAdditionalProcessUsers += '<td class="clearfix">';
            htmlAdditionalProcessUsers += '<div class="col-sm-12 pull-right">';
            htmlAdditionalProcessUsers += '<strong>' + (isMainProcess == true ? "NGƯỜI XỬ LÝ MỚI" : "NGƯỜI THAM GIA XỬ LÝ MỚI") + '</strong>';
            htmlAdditionalProcessUsers += '<span class="text-danger pull-right glyphicon glyphicon-remove" title="Xóa người xử lý mới" style="cursor:pointer" onclick="removeAllRecipients(' + isMainProcess + ')"></span></div>';
            htmlAdditionalProcessUsers += '</td>';
            htmlAdditionalProcessUsers += '</tr>'
        }
        if (isMainProcess) {
            htmlAdditionalProcessUsers += '<tr class="clearfix row-addtional">';
            htmlAdditionalProcessUsers += '<td class="clearfix">';
            htmlAdditionalProcessUsers += '<div class="col-sm-10 pull-right">';
            htmlAdditionalProcessUsers += ' <label class="radio" for="USER_MAIN_PROCESS_' + userId + '">';
            htmlAdditionalProcessUsers += '<input name="USER_MAIN_PROCESS" type="radio" value="' + userId + '" id="USER_MAIN_PROCESS_' + userId + '" class="USER_PROCESS">';
            htmlAdditionalProcessUsers += '<i></i>' + userName;
            htmlAdditionalProcessUsers += '<span class="text-danger pull-right glyphicon glyphicon-remove" title="Xóa người xử lý mới" style="cursor:pointer" onclick="removeRecipient(this,' + isMainProcess + ')"></span></label>';
            htmlAdditionalProcessUsers += '</div>';
            htmlAdditionalProcessUsers += ' </td>';
            htmlAdditionalProcessUsers += '</tr>';
        } else {
            htmlAdditionalProcessUsers += '<tr class="clearfix row-addtional">';
            htmlAdditionalProcessUsers += '<td class="clearfix">';
            htmlAdditionalProcessUsers += '<div class="col-sm-10 pull-right">';
            htmlAdditionalProcessUsers += '<label class="checkbox" for="USER_JOIN_PROCESS_' + userId + '">';
            htmlAdditionalProcessUsers += '<input name="USER_JOIN_PROCESS" type="checkbox" value="' + userId + '" id="USER_JOIN_PROCESS_' + userId + '" class="USER_PROCESS">';
            htmlAdditionalProcessUsers += '<i></i>' + userName;
            htmlAdditionalProcessUsers += '<span class="text-danger pull-right glyphicon glyphicon-remove" title="Xóa người xử lý mới" style="cursor:pointer" onclick="removeRecipient(this,' + isMainProcess + ')"></span></label>';
            htmlAdditionalProcessUsers += '</div>';
            htmlAdditionalProcessUsers += '</td>';
            htmlAdditionalProcessUsers += '</tr>';
        }
    });
    if (htmlAdditionalProcessUsers !== '') {
        if (isMainProcess) {
            $('#table--select-main-process').find('tr.row-addtional').remove();
            $('#table--select-main-process tbody tr:first').before(htmlAdditionalProcessUsers);
        } else {
            $('#table--select-join-process').find('tr.row-addtional').remove();
            $('#table--select-join-process tbody tr:first').before(htmlAdditionalProcessUsers);
        }
    }
    $('#modal--wf-additional-recipient').modal('hide');
}

function removeAllRecipients(isMainProcess) {
    $.confirm({
        'title': 'XÁC NHẬN XÓA',
        'message': 'Bạn có chắc chắn muốn xóa những người xử lý này ?',
        'buttons': {
            'Có': {
                'class': 'btn-default',
                'action': function () {
                    if (isMainProcess) {
                        $('#table--select-main-process').find('tr.row-addtional').remove();
                    } else {
                        $('#table--select-join-process').find('tr.row-addtional').remove();
                    }
                }
            },
            'Không': {
                'class': 'btn-info',
                'action': function () { }
            }
        }
    });
}

function removeRecipient(e, isMainProcess) {
    $.confirm({
        'title': 'XÁC NHẬN XÓA',
        'message': 'Bạn có chắc chắn muốn xóa người xử lý này ?',
        'buttons': {
            'Có': {
                'class': 'btn-default',
                'action': function () {
                    $(e).parents('tr.row-addtional').remove();
                    if (isMainProcess) {
                        if ($('#table--select-main-process tr.row-addtional').length == 1) {
                            $('#table--select-main-process').find('tr.row-addtional').remove();
                        }
                    } else {
                        if ($('#table--select-join-process tr.row-addtional').length == 1) {
                            $('#table--select-join-process').find('tr.row-addtional').remove();
                        }
                    }
                }
            },
            'Không': {
                'class': 'btn-info',
                'action': function () { }
            }
        }
    });
}