window.EditPostView = Backbone.View.extend({

    initialize: function () {
    },
    events: {
        "click #savePostButton": "savePost"
    },

    render: function (data) {
        $(this.el).html(this.template({ Post:data,Tags:data.tags.join(",") }));
        return this;
    },
    savePost: function (evt) {
        $("#loading").removeClass('invisible');

        var imagepath = " ";
        var isUploadingImage = false;


        var reader = new FileReader();
        reader.onload = function (event) {
            var base64 = event.target.result;

            var form = new FormData();
            form.append('data', base64);

            $.ajax({
                type: 'POST',
                url: '../api/admins/image/',
                processData: false,
                contentType: false,
                data: form,
                headers: {
                    "Authorization": "Bearer " + window.localStorage.getItem('admin-token')
                },
                success: function (data) {
                    imagepath = data;
                    isUploadingImage = false;
                },
                error: function (jqXHR, exception) {
                    $("#loading").addClass('invisible');

                    window.localStorage.removeItem('admin-token');
                    app.IsAdmin = false;
                    app.navigate("", { trigger: true, replace: true });
                }
            });



        };

        var imagemain = $("#file")[0].files;
        if (imagemain != undefined && imagemain.length != 0) {
            isUploadingImage = true;
            reader.readAsDataURL(imagemain[0]);

        }


        var postTitle = $('#title').val();
        var postSubtitle = $('#subtitle').val();
        var postAddress = $('#address').val();
        var postText = $('#summernote').summernote('code');
        var tags = $("#tags").tagsinput('items');

        if (postAddress != undefined || postAddress.length > 0) {

            var post = { EnTitle: postAddress, PostSubHeading: postSubtitle, PostTitle: postTitle, PostHtml: postText, Tags: tags }

            function waitForLoadImage() {
                if (isUploadingImage) {
                    setTimeout(waitForLoadImage, 100);
                } else {

                    post['HeaderPostImage'] = imagepath;

                    var form = new FormData();
                    form.append('post', JSON.stringify(post));

                    $.ajax({
                        type: 'PATCH',
                        url: '../api/admins/post',
                        processData: false,
                        contentType: false,
                        data: form,
                        headers: {
                            "Authorization": "Bearer " + window.localStorage.getItem('admin-token')
                        },
                        success: function (data) {
                            $("#loading").addClass('invisible');
                            app.navigate("post/" + post.EnTitle, { trigger: true, replace: true });
                        },
                        error: function (jqXHR, exception) {
                            $("#loading").addClass('invisible');

                            window.localStorage.removeItem('admin-token');
                            app.IsAdmin = false;
                            app.navigate("", { trigger: true, replace: true });
                        }
                    });
                }
            }

            waitForLoadImage();

        }
        else {
            alert('Поле адреса не может быть пустым.');
            $("#loading").addClass('invisible');
        }


        //api/admins/post


    }

});