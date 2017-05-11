var PlayerCountRadio = React.createClass({
    render: function () {
        return (
            <div className="playerCountRadio">
                <span>Title</span>
                <input type="radio" name="count" value="1" />1
                <input type="radio" name="count" value="2" />2
                <input type="radio" name="count" value="3" />3
                <input type="radio" name="count" value="4" />4
                <input type="radio" name="count" value="5" />5
            </div>
        );
    }
});

var PlayerCountText = React.createClass({
    render: function () {
        return (
            <div className="playerCountText">
                <span>Total Players:</span>
                <span>X</span>
            </div>
        );
    }
});

var GameSetup = React.createClass({
    render: function () {
        return (
            <div className="gameSetup">
                <h4>Setup Sheepshead Game</h4>
                <PlayerCountRadio />
                <PlayerCountRadio />
                <PlayerCountRadio />
                <PlayerCountRadio />
                <PlayerCountText />
                <input type="button" value="Play" />
            </div>
        );
    }
});

ReactDOM.render(
    <GameSetup />,
    document.getElementById('content')
);