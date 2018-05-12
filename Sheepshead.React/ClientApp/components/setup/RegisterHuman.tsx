import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface RegisterHumanState {
    gameId: string;
    playerName: string;
}

export class RegisterHuman extends React.Component<RouteComponentProps<{}>, RegisterHumanState> {
    readonly MAX_PLAYERS = 5;
    readonly HUMANS = "humanCount";
    readonly NEWBIE = "newbieCount";
    readonly BASIC = "basicCount";

    constructor() {
        super();
        this.state = { gameId: '', playerName: '' };
        this.handleNameChange = this.handleNameChange.bind(this);
    }

    handleNameChange(e: React.FormEvent<HTMLInputElement>) {
        this.setState({ playerName: e.currentTarget.value });
    }

    validName() {
        return false;
    }

    public render() {
        return (
            <div className="gameSetup">
                <h4>Register Player</h4>
                <form method="post">
                    <label>Your name</label>
                    <input type="text" name="playerName" onChange={this.handleNameChange}/>
                    <input type="hidden" name="gameId" value={this.state.gameId} />
                    <input type="submit" value="Play" disabled={this.validName()} />
                </form>
                Share this URL with your friends to allow them to join the game.
            </div>
        );
    }
}