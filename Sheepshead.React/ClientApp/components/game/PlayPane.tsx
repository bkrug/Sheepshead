import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import GameDetails from './GameDetails';
import ActionPane from './ActionPane';

export interface PlayPaneState {
    gameId: string;
    trickWinners: string[];
}

export class PlayPane extends React.Component<RouteComponentProps<{}>, PlayPaneState> {
    constructor(props: any) {
        super(props);
        this.state = {
            gameId: IdUtils.getGameId(props),
            trickWinners: []
        };
        this.trickEnd = this.trickEnd.bind(this);
    }

    private trickEnd(): void {
        var self = this;
        FetchUtils.get(
            'Game/GetTrickResults?gameId=' + self.state.gameId,
            function (json: any): void {
                self.setState({
                    trickWinners: json.trickWinners
                });
            });
    }

    public render() {
        return (
            <div className="playPane">
                <GameDetails gameId={this.state.gameId} />
                <div>
                    <h4>Hand Details</h4>
                    {
                        this.state.trickWinners.map((playerName: string, i: number) =>
                            <div key={i}><b>Trick {i+1}</b> {playerName}</div>
                        )
                    }
                </div>
                <ActionPane gameId={this.state.gameId} onTrickEnd={this.trickEnd} />
            </div>
        );
    }
}