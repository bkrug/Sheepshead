﻿import React from 'react';
import 'whatwg-fetch';
import { withQuery } from './withQuery'; 
import PlayerCountRadio from './PlayerCountRadio';

export default class GameSetup extends React.Component {
    constructor(props) {
        super(props);
        this.setConstants();
        this.state = { value: 0, remaining: this.MAX_PLAYERS };
        this.handleChange = this.handleChange.bind(this);
        this.handlePlayClick = this.handlePlayClick.bind(this);
        this.selections = {};
        this.selections[this.HUMANS] = this.selections[this.NEWBIE] = this.selections[this.BASIC] = this.selections[this.LEARNING] = 0;
    }

    setConstants() {
        this.MAX_PLAYERS = 5;
        this.HUMANS = "humans";
        this.NEWBIE = "newbie";
        this.BASIC = "basic";
        this.LEARNING = "learning";
    }

    handleChange(radioGroup, radioValue) {
        if (radioValue === 0 || radioValue > 0) {
            this.selections[radioGroup] = radioValue;
            var newTotal = this.selections[this.HUMANS] + this.selections[this.NEWBIE] + this.selections[this.BASIC] + this.selections[this.LEARNING];
            this.setState({ value: newTotal, remaining: this.MAX_PLAYERS - newTotal });
        }
    }

    playerCountValidityClass() {
        return (this.state.value === 3 || this.state.value === 5) ? '' : 'invalidcount';
    }

    handlePlayClick() {
        var gameStartModel = {
            HumanCount: this.selections[this.HUMANS],
            NewbieCount: this.selections[this.NEWBIE],
            BasicCount: this.selections[this.BASIC],
            LearningCount: this.selections[this.LEARNING]
        };
        fetch(
            'http://localhost:61904/api/game/create',
            {
                method: 'POST',
                body: JSON.stringify(gameStartModel),
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                }
            }
        )
        .then(function (response) {
            return response.json();
        }).then(function (json) {
            console.log('parsed json', json)
            if (json.success === true)
                document.location = "/Shuffle";
            else
                throw "Unknown server-side exception occurred.";
        }).catch(function (ex) {
            console.log('parsing failed', ex)
        });
    }

    render() {
        return (
            <div className="gameSetup">
                <h4>Setup Sheepshead Game</h4>
                <PlayerCountRadio name={this.HUMANS} title="Humans" onChange={this.handleChange} remaining={this.state.remaining} />
                <PlayerCountRadio name={this.NEWBIE} title="A.I. Simple" onChange={this.handleChange} remaining={this.state.remaining} />
                <PlayerCountRadio name={this.BASIC} title="A.I. Basic" onChange={this.handleChange} remaining={this.state.remaining} />
                <PlayerCountRadio name={this.LEARNING} title="A.I. Statistic" onChange={this.handleChange} remaining={this.state.remaining} />
                <div>
                    <span>Total Players:</span>
                    <span className={"totalPlayers " + this.playerCountValidityClass()}>{this.state.value}</span>
                </div>
                <input type="hidden" className="remaining" value={this.state.remaining} />
                <input type="button" value="Play" onClick={this.handlePlayClick} />
            </div>
        );
    }
}