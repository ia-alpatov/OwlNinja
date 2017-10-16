window.AdminView = Backbone.View.extend({

    initialize:function () {

    },
    events: {
        "click #sendMessageButton": "sendAuth"
    },
    render: function () {
        $(this.el).html(this.template());
    },
    sendAuth: function (evt) {
        var username = $('#username').val();
        var password = $('#password').val();

        if (!username || username.length <= 3) {
            $('#username_label').addClass('alert-danger');
        }
        else
        {
            $('#username_label').removeClass('alert-danger');
            if (!password || password.length <= 3) {
                $('#password_label').addClass('alert-danger');
            }
            else {
                $('#password_label').removeClass('alert-danger');
                var recaptcha = grecaptcha.getResponse(window.currentGrecaptchaId);
                if (!recaptcha || recaptcha.length <= 3) {
                    $('#AuthResult').html("Решите reCAPTCHA!");
                }
                else
                {
                    $('#sendMessageButton').html("<i class='fa fa-spinner fa-spin'></i> Подождите");
                    $('#sendMessageButton').addClass('disabled');
                    $('#sendMessageButton').attr('disabled', 'disabled');

                    $('#username_label').removeClass('alert-danger');
                    $('#password_label').removeClass('alert-danger');
                    $('#AuthResult').html("");

                    var form = new FormData();
                    form.append('username', username);
                    form.append('password', password);
                    form.append('g-recaptcha-response', recaptcha);

                    $("#loading").removeClass('invisible');

                    $.ajax({
                        type: 'POST',
                        url: '../api/admins/auth',
                        data: form,
                        processData: false,
                        contentType: false,
                        success: function (data) {
                            $("#loading").addClass('invisible');

                            window.localStorage.setItem('admin-token', data);
                            app.AdminToken = data;
                            app.IsAdmin = true;
                            app.navigate("settings", { trigger: true, replace: true });
                        },
                        error: function (jqXHR, exception) {
                            grecaptcha.reset(window.currentGrecaptchaId);

                            if (jqXHR.status == 404) {
                                $('#AuthResult').html("Проверьте логин и пароль.");
                            } else{
                                $('#AuthResult').html("Неизвестная ошибка.");
                            } 

                            $('#sendMessageButton').html("Войти");
                            $('#sendMessageButton').removeClass('disabled');
                            $('#sendMessageButton').removeAttr('disabled');

                            $("#loading").addClass('invisible');
                        },
                    });
                }
            }
        }
    }
});
