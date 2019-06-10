var form = {
    /**
    * Tu dong focus vao truong dau tien cua formid ma co the edit
    **/
    focusForm: function (formId) {
        $("#" + formId + " :input:visible:enabled:first").focus();
    },

    resetForm: function (containerId) {
        $("#" + containerId).find(":input").each(function () {
            var type = this.type;
            var tag = this.tagName.toLowerCase(); // normalize case

            if (type == 'text' || type == 'password' || tag == 'textarea')
                this.value = "";

            else if (type == 'checkbox' || type == 'radio')
                this.checked = false;

            else if (tag == 'select') {
                this.selectedIndex = 0;
            }
        });
        var inputs = $("#" + containerId).find(".input-validation-error");
        var msgs = $("#" + containerId).find(".field-validation-error");
        inputs.removeAttr("class");
        //truong hop dac biet Namta
        //nut button disabled

        $("#" + containerId + " .t-state-disabled").each(function () {
            $(this).attr("disabled", "");
        });

        $("#" + containerId + " button:disabled").each(function () {
            if (!$(this).hasClass(".t-state-disabled")) {
                $(this).addClass(".t-state-disabled");
            }
        });
        //disable duong dan
        $("#txtFilePath").attr("disabled", "");

        msgs.removeClass("field-validation-error");
        msgs.addClass("field-validation-valid");
        msgs.children().remove();
    },


    resetClassForm: function (classFormName) {
        $("." + classFormName).find(":input").each(function () {
            var type = this.type;
            var tag = this.tagName.toLowerCase(); // normalize case

            if (type == 'text' || type == 'password' || tag == 'textarea')
                this.value = "";

            else if (type == 'checkbox' || type == 'radio')
                this.checked = false;

            else if (tag == 'select') {
                this.selectedIndex = 0;
            }
        });
        var inputs = $("." + classFormName).find(".input-validation-error");
        var msgs = $("." + classFormName).find(".field-validation-error");
        inputs.removeAttr("class");

        msgs.removeClass("field-validation-error");
        msgs.addClass("field-validation-valid");
        msgs.children().remove();
    },


    resetFormFixCombo: function (containerId) {
        $("#" + containerId).find(":input").each(function () {
            var type = this.type;
            var tag = this.tagName.toLowerCase(); // normalize case

            if (type == 'text' || type == 'password' || tag == 'textarea')
                this.value = "";

            else if (type == 'checkbox' || type == 'radio')
                this.checked = false;

            else if (tag == 'select') {
                this.selectedIndex = 0;
                this.text = '[Lựa chọn]';
            }
        });
        var inputs = $("#" + containerId).find(".input-validation-error");
        var msgs = $("#" + containerId).find(".field-validation-error");
        inputs.removeAttr("class");

        msgs.removeClass("field-validation-error");
        msgs.addClass("field-validation-valid");
        msgs.children().remove();
    },
    //ham xoa bo toan bo validate namta
    resetValidate: function (containerId) {

        var inputs = $("#" + containerId).find(".input-validation-error");
        var msgs = $("#" + containerId).find(".field-validation-error");
        inputs.removeAttr("class");

        msgs.removeClass("field-validation-error");
        msgs.addClass("field-validation-valid");
        msgs.children().remove();
    }
};