import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import GameDetails from './GameDetails';
import ActionPane from './ActionPane';
import { GameScore } from 'ClientApp/components/game/PlayState';

export interface PlayPaneState {
    gameId: string;
    picker: string;
    partner: string;
    trickWinners: string[];
    coins: GameScore[];
}

export class PlayPane extends React.Component<RouteComponentProps<{}>, PlayPaneState> {
    constructor(props: any) {
        super(props);
        this.state = {
            gameId: IdUtils.getGameId(props),
            picker: '',
            partner: '',
            trickWinners: [],
            coins: []
        };
        this.trickEnd = this.trickEnd.bind(this);
        this.handEnd = this.handEnd.bind(this);
    }

    private handEnd(): void {
        var self = this;
        FetchUtils.get(
            'Game/GameSummary?gameId=' + self.state.gameId,
            function (json: GameScore[]): void {
                self.setState({
                    coins: json,
                    picker: '',
                    partner: '',
                    trickWinners: []
                });
            });
    }

    private trickEnd(): void {
        var self = this;
        FetchUtils.get(
            'Game/GetTrickResults?gameId=' + self.state.gameId,
            function (json: any): void {
                self.setState({
                    picker: json.picker,
                    partner: json.partner,
                    trickWinners: json.trickWinners
                });
            });
    }

    public render() {
        return (
            <div className="playPane">
                <div>
                    <h4>Game Details</h4>
                    {
                        this.state.coins.map((coinScore: GameScore, i: number) =>
                            <div key={i} style={{ display: 'inline-flex', margin: '0px 20px', textAlign: 'center' }}>
                                {coinScore.name}
                                <br />
                                {coinScore.score || '-'}
                            </div>
                        )
                    }
                </div>
                <div>
                    <h4>Hand Details</h4>
                    <div>Picker: {this.state.picker}</div>
                    <div>Partner: {this.state.partner}</div>
                    {
                        this.state.trickWinners.map((playerName: string, i: number) =>
                            <div key={i}><b>Trick {i+1}</b> {playerName}</div>
                        )
                    }
                </div>
                <ActionPane gameId={this.state.gameId}
                    onHandEnd={this.handEnd}
                    onTrickEnd={this.trickEnd} />
            </div>
        );
    }
}