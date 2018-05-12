import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface RegisterHumanState {
    gameId: string;
    playerName: string;
}

export class RegisterHuman extends React.Component<RouteComponentProps<{}>, RegisterHumanState> {
    constructor(props: any) {
        super(props);
        this.state = { gameId: this.getGameId(props), playerName: '' };
        this.handleNameChange = this.handleNameChange.bind(this);
    }

    private getGameId(props: any) {
        var pathParts = props.location.pathname.split('/');
        var indexOfGameId = pathParts.indexOf('RegisterHuman') + 1;
        var gameId = pathParts[indexOfGameId];
        return gameId;
    }

    private handleNameChange(e: React.FormEvent<HTMLInputElement>) {
        this.setState({ playerName: e.currentTarget.value });
    }

    private validName() {
        return this.state.playerName ? true : false;
    }

    public render() {
        return (
            <div>
                <h4>Register Player</h4>
                Share this page's URL with your friends to allow them to join the game.
                <form method="post">
                    <label>Your name</label>
                    <input type="text" name="playerName" onChange={this.handleNameChange}/>
                    <input type="hidden" name="gameId" value={this.state.gameId} />
                    <input type="submit" value="Play" disabled={!this.validName()} />
                </form>
            </div>
        );
    }
}