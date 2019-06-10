function processWF(entity_item_id, entity_id, trang_thai_one, trang_thai_two, action, call_back_function, business_class) {
    $("#HIDDEN_ENTITY_ITEM_ID").val("");
    $("#HIDDEN_ENTITY_ID").val("");
    $("#HIDDEN_TRANG_THAI_ONE").val("");
    $("#HIDDEN_TRANG_THAI_TWO").val("");
    $("#HIDDEN_CALL_BACK_FUNCTION").val("");
    $("#HIDDEN_ACTION").val("");
    $("#HIDDEN_BUSINESS_CLASS").val("");

    $("#myModalLabel").html('<span style="font-weight: bold; font-size: 20px;">' + action + '</span>');
    $("#comment_text_step_id").val("");
    $("#ERR_comment_text_step_id").html("");

    $("#HIDDEN_ENTITY_ITEM_ID").val(entity_item_id);
    $("#HIDDEN_ENTITY_ID").val(entity_id);
    $("#HIDDEN_TRANG_THAI_ONE").val(trang_thai_one);
    $("#HIDDEN_TRANG_THAI_TWO").val(trang_thai_two);
    $("#HIDDEN_CALL_BACK_FUNCTION").val(call_back_function);
    $("#HIDDEN_ACTION").val(action);
    $("#HIDDEN_BUSINESS_CLASS").val(business_class);

    $("#ProcessWorkFlowId").modal('show');

}
function SubmitToFinishChangeStep() {
    var entity_item_id = $("#HIDDEN_ENTITY_ITEM_ID").val().trim();
    var entity_id = $("#HIDDEN_ENTITY_ID").val().trim();
    var trang_thai_one = $("#HIDDEN_TRANG_THAI_ONE").val().trim();
    var trang_thai_two = $("#HIDDEN_TRANG_THAI_TWO").val().trim();
    var action = $("#HIDDEN_ACTION").val().trim();
    var call_back_function = $("#HIDDEN_CALL_BACK_FUNCTION").val().trim();
    var business_class = $("#HIDDEN_BUSINESS_CLASS").val().trim();
    var message = $("#comment_text_step_id").val().trim();

    if (message == "") {
        $("#ERR_comment_text_step_id").html("Bạn phải nhập nội dung " + action);
    } else {
        if (entity_item_id == "" || entity_id == "" || trang_thai_one == "" || trang_thai_two == "") {
            $(".ui-dialog-content").dialog("close");
            $("#ProcessWorkFlowId").modal('hide');
            notif({
                type: 'error',
                position: 'bottom',
                msg: 'Đã có lỗi xảy ra!',
            });
        } else {
            $(".ui-dialog-content").dialog("close");
            $("#ProcessWorkFlowId").modal('hide');
            $.ajax({
                url: '/WorkFlowArea/WorkFlow/ProcessStep',
                type: 'post',
                cache: false,
                data: {
                    ENTITY_ITEM_ID: entity_item_id,
                    ENTITY_ID: entity_id,
                    TRANG_THAI_ONE: trang_thai_one,
                    TRANG_THAI_TWO: trang_thai_two,
                    BUSINESS_CLASS: business_class,
                    MESSAGE: message
                },
                success: function (data) {
                    notif({
                        type: 'success',
                        position: 'bottom',
                        msg: action + ' thành công!',
                    });
                    if (call_back_function.length > 0) {
                        var cb_fn = window[call_back_function];
                        cb_fn();
                    }
                },
                error: function (err) {
                    notif({
                        type: 'error',
                        position: 'bottom',
                        msg: action + ' không thành công!',
                    });
                }
            });           
        }
    }
    

}