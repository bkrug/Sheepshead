$(document).ready(function() 
{
    var pickPage = new PickPage();
    $('#no-btn').click(pickPage.OnNoClick);
    $('#yes-btn').click(pickPage.OnYesClick);
});

var PickPage = function () {
    this.OnNoClick = function (e) {
        $('#id').val(1);
        $('#willPick').val(false);
        $('form').submit();
    };

    this.OnYesClick = function (e) {
        $('#id').val(1);
        $('#willPick').val(true);
        $('form').submit();
    };
};