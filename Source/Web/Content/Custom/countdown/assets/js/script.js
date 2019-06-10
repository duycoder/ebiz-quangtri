$(function () {

    var note = $('#note'),
		ts = new Date(2016, 4, 20,00,00,00,00),
		newYear = true;

    if ((new Date()) > ts) {
        // The new year is here! Count towards something else.
        // Notice the *1000 at the end - time must be in milliseconds
        ts = (new Date()).getTime() + 10 * 24 * 60 * 60 * 1000;
        newYear = false;
    }

    $('#countdown').countdown({
        timestamp: ts,
        callback: function (days, hours, minutes, seconds) {

            var message = "Còn lại: ";

            message += days + " ngày, ";
            message += hours + " giờ, ";
            message += minutes + " phút và ";
            message += seconds + " giây <br />";

            //if (newYear) {
            //    message += "còn lại cho đến năm mới!";
            //}
            //else {
            //    message += "còn lại đến 10 ngày kể từ bây giờ!";
            //}

            note.html(message);
        }
    });

});
