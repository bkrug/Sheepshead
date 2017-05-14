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