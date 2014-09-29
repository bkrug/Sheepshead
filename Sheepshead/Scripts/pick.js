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
        var dropCards = [];
        $('.js-card-cb').each(function () {
            if ($(this).is(':checked'))
                dropCards.push($(this).data('index'));
        });
        if (dropCards.length == 2) {
            $('#id').val(1);
            $('#willPick').val(true);
            $('#droppedCardIndicies').val(dropCards.join(';'));
            $('form').submit();
        }
        else {
            alert("Must drop exactly two cards.");
        }
    };
};