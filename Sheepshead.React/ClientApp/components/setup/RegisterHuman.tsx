import '../../css/setup.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';

export interface RegisterHumanState {
    gameId: string;
    playerName: string;
}

export class RegisterHuman extends React.Component<RouteComponentProps<{}>, RegisterHumanState> {
    constructor(props: any) {
        super(props);
        this.state = { gameId: IdUtils.getGameId(props), playerName: '' };
        this.handleNameChange = this.handleNameChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    private handleNameChange(e: React.FormEvent<HTMLInputElement>) {
        this.setState({ playerName: e.currentTarget.value });
    }

    private validName() {
        return this.state.playerName ? true : false;
    }

    private handleSubmit(e: React.FormEvent<HTMLInputElement>) {
        var gameId = this.state.gameId;
        var self = this;
        FetchUtils.post('Setup/RegisterHuman?gameId=' + this.state.gameId + '&playerName=' + this.state.playerName,
            function (json: any) {
                IdUtils.setPlayerId(gameId, json.playerId);
                window.location.href = (json.full)
                    ? '/setup/gamefull/' + gameId
                    : '/setup/registrationwait/' + gameId;
            });
    }

    public render() {
        return (
            <div className="page-contents">
                <div className="centered-page-contents">
                    <h4>Register Player</h4>
                    <p>Share this page's URL with your friends to allow them to join the game.</p>
                    <label>Your name</label>
                    <input type="text" name="playerName" onChange={this.handleNameChange}/>
                    <input type="hidden" name="gameId" value={this.state.gameId} />
                    <input type="button" value="Play" disabled={!this.validName()} onClick={this.handleSubmit} />
                </div>
            </div>
        );
    }
}