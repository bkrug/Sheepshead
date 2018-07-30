import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import ActionPane from './ActionPane';
import { GameScore } from 'ClientApp/components/game/PlayState';
import { CheatSheetModal } from './CheatSheetModal';

export interface PlayPaneState {
    gameId: string;
    picker: string;
    partner: string;
    partnerCard: string;
    trickWinners: string[];
    coins: GameScore[];
    leastersHand: boolean;
}

export class PlayPane extends React.Component<RouteComponentProps<{}>, PlayPaneState> {
    constructor(props: any) {
        super(props);
        this.state = {
            gameId: IdUtils.getGameId(props),
            picker: '',
            partner: '',
            partnerCard: '',
            trickWinners: [],
            coins: [],
            leastersHand: false
        };
        this.trickEnd = this.trickEnd.bind(this);
        this.handEnd = this.handEnd.bind(this);
        this.handEnd();
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
                    partnerCard: '',
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
                    partnerCard: json.partnerCard,
                    trickWinners: json.trickWinners,
                    leastersHand: json.leastersHand
                });
            });
    }

    public render() {
        return (
            <div className="playPane">
                <CheatSheetModal />
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
                    <div>Partner Card: {this.state.partnerCard}</div>
                    {this.state.leastersHand ? <b>Leasters Hand</b> : ''}
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