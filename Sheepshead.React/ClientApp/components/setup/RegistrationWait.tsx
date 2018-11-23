import '../../css/setup.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ReactDOM from 'react-dom';
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

    private copyToClipboard() {
        var copyTextarea = document.querySelector('textarea');
        if (copyTextarea == null)
            copyTextarea = new HTMLTextAreaElement();
        copyTextarea.select();

        try {
            var successful = document.execCommand('copy');
            var msg = successful ? 'successful' : 'unsuccessful';
            console.log('Copying text command was ' + msg);
        } catch (err) {
            console.log('Oops, unable to copy');
        }
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
                        <p>Share this url with friends that you will play with:</p>
                        <textarea disabled={false}>{this.getUrl()}</textarea>
                        <button onClick={this.copyToClipboard}>Copy URL to clipboard</button>
                    </div>
                    <div hidden={!this.state.allPlayersReady}>
                        All players have registered.
                    </div>
                </div>
            </div>
        );
    }
}