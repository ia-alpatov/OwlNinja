window.SettingsView = Backbone.View.extend({

    initialize: function () {
    },

    render: function (settings) {
        $(this.el).html(this.template(settings));
        return this;
    }

});