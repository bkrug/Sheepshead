import React from 'react';

export default class PlayerCountRadio extends React.Component {
    render() {
        return (
            <div className="playerCountRadio">
                <span>{this.props.title}</span>
                <input type="radio" name={this.props.name} value="0" checked="checked" />0
                <input type="radio" name={this.props.name} value="1" />1
                <input type="radio" name={this.props.name} value="2" />2
                <input type="radio" name={this.props.name} value="3" />3
                <input type="radio" name={this.props.name} value="4" />4
                <input type="radio" name={this.props.name} value="5" />5
            </div>
        );
    }
}

class PlayerCountText extends React.Component {
    render() {
        return (
            <div className="playerCountText">
                <span>Total Players:</span>
                <span>0</span>
            </div>
        );
    }
}

class GameSetup extends React.Component {
    render() {
        return (
            <div className="gameSetup">
                <h4>Setup Sheepshead Game</h4>
                <PlayerCountRadio name="humans" title="Humans" />
                <PlayerCountRadio name="newbie" title="A.I. Simple" />
                <PlayerCountRadio name="basic" title="A.I. Basic" />
                <PlayerCountRadio name="learning" title="A.I. Statistic" />
                <PlayerCountText />
                <input type="button" value="Play" />
            </div>
        );
    }
}