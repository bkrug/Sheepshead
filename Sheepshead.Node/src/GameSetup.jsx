import React from 'react';
import PlayerCountRadio from './PlayerCountRadio';

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

export default class GameSetup extends React.Component {
    totalValue() {
        return null;
    }
    onChange(playerCountRadio) {

    }
    render() {
        return (
            <div className="gameSetup">
                <h4>Setup Sheepshead Game</h4>
                <PlayerCountRadio name="humans" title="Humans" onChange={this.onChange} />
                <PlayerCountRadio name="newbie" title="A.I. Simple" onChange={this.onChange} />
                <PlayerCountRadio name="basic" title="A.I. Basic" onChange={this.onChange} />
                <PlayerCountRadio name="learning" title="A.I. Statistic" onChange={this.onChange} />
                <span>{this.totalValue()}</span>
                <PlayerCountText />
                <input type="button" value="Play" />
            </div>
        );
    }
}