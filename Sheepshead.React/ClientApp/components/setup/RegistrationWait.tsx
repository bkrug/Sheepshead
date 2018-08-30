import '../../css/setup.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ReactDOM from 'react-dom';
import CopyToClipboard from 'react-copy-to-clipboard';
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
                if (data.allPlayersReady) {
                    this.setState({ allPlayersReady: true });
                    window.location.href = '/game/playpane/' + this.state.gameId;
                }
                else
                    setTimeout(() => { this.checkIfPlayersReady() }, 1000)
            });
    }

    public render() {
        return (
            <div className="page-contents">
                <div className="centered-page-contents">
                    <h4>Play Sheepshead</h4>
                    <div hidden={this.state.allPlayersReady}>
                        <p>Waiting for other players.</p>
                        <p>Share this url with friends that you will play with: <b>{this.getUrl()}</b> </p>
                        <CopyToClipboard text={this.getUrl()}>
                            <button>Copy URL to clipboard</button>
                        </CopyToClipboard>
                    </div>
                    <div hidden={!this.state.allPlayersReady}>
                        All players have registered.
                    </div>
                </div>
            </div>
        );
    }
}