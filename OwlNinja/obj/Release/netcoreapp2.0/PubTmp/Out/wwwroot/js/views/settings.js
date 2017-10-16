window.SettingsView = Backbone.View.extend({

    initialize: function () {
    },
    events: {
        "click #createPostButton": "createPost",
        "click #saveSettingsButton":"saveSettings"
    },
    render: function (settings) {
        $(this.el).html(this.template(settings));
        return this;
    },
    createPost: function () {
       app.navigate("createpost", { trigger: true, replace: true });
    },
    saveSettings: function (evt) 
    {
        $("#loading").removeClass('invisible');

        var currentName = "";
        var isUploadingImage = false;
        var settingsToSave = app.Settings;

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
                    settingsToSave[currentName] = data;
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

        var imagemain = $("#imagemain")[0].files;
        if (imagemain != undefined && imagemain.length != 0) {
            currentName = 'homeBgUrl';
            isUploadingImage = true;
            reader.readAsDataURL(imagemain[0]);
        }

        var imagetags = $("#imagetags")[0].files;
        if (imagetags != undefined && imagetags.length != 0) {
            currentName = 'postsByTagsBgUrl';
            isUploadingImage = true;
            reader.readAsDataURL(imagetags[0]);
        }

        var imageAboutMe = $("#imageAboutMe")[0].files;
        if (imageAboutMe != undefined && imageAboutMe.length != 0) {
            currentName = 'aboutMeBgUrl';
            isUploadingImage = true;
            reader.readAsDataURL(imageAboutMe[0]);
        }        

        var imagePost = $("#imagePost")[0].files;
        if (imagePost != undefined && imagePost.length != 0) {
            currentName = 'createPostImage';
            isUploadingImage = true;
            reader.readAsDataURL(imagePost[0]);
        } 


        var imageSettings = $("#imageSettings")[0].files;
        if (imageSettings != undefined && imageSettings.length != 0) {
            currentName = 'settingsImage';
            isUploadingImage = true;
            reader.readAsDataURL(imageSettings[0]);
        } 

        var imageAdmin = $("#imageAdmin")[0].files;
        if (imageAdmin != undefined && imageAdmin.length != 0) {
            currentName = 'adminImage';
            isUploadingImage = true;
            reader.readAsDataURL(imageAdmin[0]);
        }
        


        var titlemain = $('#titlemain').val();
        var subtitlemain = $('#subtitlemain').val();
        var titletags = $('#titletags').val();
        var titleAboutMe = $('#titleAboutMe').val();
        var subtitleAboutMe = $('#subtitleAboutMe').val();

        var summernoteAboutMe = $('#summernoteAboutMe').summernote('code');
        
        settingsToSave['homeTitle'] = titlemain;
        settingsToSave['homeSubHeading'] = subtitlemain;
        settingsToSave['postsByTagsTitle'] = titletags;
        settingsToSave['aboutMeTitle'] = titleAboutMe;
        settingsToSave['aboutMeSubHeading'] = subtitleAboutMe;
        settingsToSave['aboutMeHtml'] = summernoteAboutMe;

        function waitForLoadImage() {
            if (isUploadingImage) {
                setTimeout(waitForLoadImage, 100);
            } else {

                var form = new FormData();
                form.append('sets', JSON.stringify(settingsToSave));
                
                $.ajax({
                    type: 'PUT',
                    url: '../api/admins/settings/',
                    processData: false,
                    contentType: false,
                    data: form,
                    headers: {
                        "Authorization": "Bearer " + window.localStorage.getItem('admin-token')
                    },
                    success: function (data) {
                       app.Settings = settingsToSave;
                       $("#loading").addClass('invisible');
                       app.navigate("settings", { trigger: true, replace: true });
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

});