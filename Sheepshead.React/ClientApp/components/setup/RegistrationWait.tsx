import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';

export interface RegistrationWaitState {
    gameId: string;
    allPlayersReady: boolean;
}

export class RegistrationWait extends React.Component<RouteComponentProps<{}>, RegistrationWaitState> {
    constructor(props: any) {
        super(props);
        this.state = {
            gameId: IdUtils.getGameId(props),
            allPlayersReady: false
        };
        this.getUrl = this.getUrl.bind(this);
        this.checkIfPlayersReady();
    }

    private getUrl() {
        return window.location.host + '/setup/RegisterHuman/' + this.state.gameId;
    }

    private checkIfPlayersReady() {
        fetch('/Setup/AllPlayersReady?gameId=' + this.state.gameId)
            .then(response => response.json())
            .then(data => {
                if (data.allPlayersReady)
                    this.setState({ allPlayersReady: true });
                else
                    setTimeout(() => { this.checkIfPlayersReady() }, 1000)
            });
    }

    public render() {
        return (
            <div>
                <h4>Play Sheepshead</h4>
                <div hidden={this.state.allPlayersReady}>
                    <p>Waiting for other players.</p>
                    <p>Share this url with friends that you will play with: <b>{this.getUrl()}</b> </p>
                </div>
                <div hidden={!this.state.allPlayersReady}>
                    All players have registered.
                </div>
            </div>
        );
    }
}