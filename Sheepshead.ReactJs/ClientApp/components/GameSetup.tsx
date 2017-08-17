import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'whatwg-fetch';
//import { withQuery } from './withQuery';
import PlayerCountRadio from './PlayerCountRadio';

export interface GameSetupState {
    value: number;
    remaining: number;
    gameName: string;
}

export class GameSetup extends React.Component<any, any> {
    selections: { [index: string]: number };
    MAX_PLAYERS = 5;
    HUMANS: "humans";
    NEWBIE: "newbie";
    BASIC: "basic";
    LEARNING: "learning";

    constructor() {
        super();
        this.state = { value: 0, remaining: this.MAX_PLAYERS, gameName: '' };
        this.handleChange = this.handleChange.bind(this);
        this.handleNameChange = this.handleNameChange.bind(this);
        this.handlePlayClick = this.handlePlayClick.bind(this);
        this.selections = { "a": 0 };
        this.selections[this.HUMANS] = 0;
        this.selections[this.NEWBIE] = this.selections[this.BASIC] = this.selections[this.LEARNING] = 0;
        //this.headerTest();
    }

    //headerTest() {
    //    fetch(
    //        withQuery('http://localhost:61904/api/game/RequestPlayerKey', { oldKey: '1ad8bd99-951d-4d93-a502-8eb0e6ee0a31' }),
    //        {
    //            method: 'POST',
    //            headers: {
    //                'Accept': 'application/json',
    //                'Content-Type': 'application/json'
    //            }
    //        }
    //    )
    //    .then(function (response) {
    //        return response.json();
    //    }).then(function (json) {
    //        console.log('parsed json', json);
    //        localStorage.setItem('player-key', json);
    //        return json;
    //    });
    //}

    //headerTest2() {
    //    var playerKey = localStorage.getItem('player-key');
    //    if (!playerKey)
    //        throw 'player key was not recorded';
    //    console.log('player key ' + playerKey);
    //    fetch(
    //        'http://localhost:61904/api/game/ValidatePlayerKey?someNumber=5',
    //        {
    //            method: 'POST',
    //            headers: {
    //                'Accept': 'application/json',
    //                'Content-Type': 'application/json',
    //                'Player-Key': playerKey
    //            }
    //        }
    //    )
    //    .then(function (response) {
    //        return response.json();
    //    }).then(function (json) {
    //        console.log('parsed json', json);
    //    });
    //}

    handleChange(radioGroup: string, radioValue: number) {
        //this.headerTest2();
        if (radioValue === 0 || radioValue > 0) {
            this.selections[radioGroup] = radioValue;
            var newTotal = this.selections[this.HUMANS] + this.selections[this.NEWBIE] + this.selections[this.BASIC] + this.selections[this.LEARNING];
            this.setState({ value: newTotal, remaining: this.MAX_PLAYERS - newTotal });
        }
    }

    handleNameChange(e: React.FormEvent<HTMLInputElement>) {
        console.log(e.currentTarget.value);
        this.setState({ gameName: e.currentTarget.value });
    }

    playerCountValidityClass() {
        return (this.state.value === 3 || this.state.value === 5) ? '' : 'invalidcount';
    }

    handlePlayClick() {
        var gameStartModel = {
            GameName: this.state.gameName,
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
            })
            .then(function (json) {
                console.log('parsed json', json)
                if (json.success === true) { }
                    //document.location = "/Shuffle";
                else
                    throw new Error("Unknown server-side error occurred.");
            }).catch(function (ex) {
                console.log('parsing failed', ex)
            });
    }

    public render() {
        return (
            <div className="gameSetup">
                <h4>Setup Sheepshead Game</h4>
                <label>Game Name</label>
                <input id="name-input" type="text" onChange={ e => this.handleNameChange(e) } value={this.state.gameName} />
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