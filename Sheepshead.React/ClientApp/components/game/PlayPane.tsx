import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import ActionPane from './ActionPane';
import { GameScore, CardSummary } from 'ClientApp/components/game/PlayState';
import { CheatSheetModal } from './CheatSheetModal';

export interface PlayPaneState {
    gameId: string;
    picker: string;
    partner: string;
    partnerCard: string;
    trickWinners: string[];
    coins: GameScore[];
    leastersHand: boolean;
    tricks: { key: string, value: CardSummary[] }[];
    showGroupedTricks: boolean;
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
            leastersHand: false,
            tricks: [],
            showGroupedTricks: false
        };
        this.trickEnd = this.trickEnd.bind(this);
        this.handEnd = this.handEnd.bind(this);
        this.showGroupedTricks = this.showGroupedTricks.bind(this);
        this.hideGroupedTricks = this.hideGroupedTricks.bind(this);
        this.renderModal = this.renderModal.bind(this);
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
                    partnerCard: ''
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
                    leastersHand: json.leastersHand,
                    tricks: json.tricks
                });
            });
    }

    private showGroupedTricks(): void {
        if (this.state.tricks.length > 0 && this.state.tricks[0].value.length > 0)
            this.setState({
                showGroupedTricks: true
            });
    }

    private hideGroupedTricks(): void {
        this.setState({
            showGroupedTricks: false
        });
    }

    private renderModal() {
        var self = this;
        var playerList = this.state.tricks.map((trick: { key: string, value: CardSummary[] }, i: number) =>
            <div key={i} className='trick-summary'>
                <p>{trick.key}</p>
                {trick.value.map((cardSummary: CardSummary, j: number) =>
                    <p key={j} className={this.getCssClass(cardSummary)}>{cardSummary.abbreviation}</p>
                )}
            </div>
        );
        return (
            <div className="modal-dialog trick-cards">
                <div>
                    {playerList}
                </div>
            </div>
        );
    }

    private getCssClass(cardSummary: CardSummary): string {
        return cardSummary.abbreviation.indexOf('♥') >= 0 || cardSummary.abbreviation.indexOf('♦') >= 0 ? 'redCard' : 'blkCard';
    }

    public render() {
        return (
            <div className="play-pane page-contents">
                <div className="centered-page-contents">
                    <CheatSheetModal />
                    <div className="game-details">
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
                    {this.state.showGroupedTricks ? this.renderModal() : <div></div> }
                    <div>
                        <h4>Hand Details</h4>
                        <div className={'hand-data'}>
                            <div>Picker: {this.state.picker}</div>
                            <div>Partner: {this.state.partner}</div>
                            <div>Partner Card: {this.state.partnerCard}</div>
                            {this.state.leastersHand ? <b>Leasters Hand</b> : ''}
                        </div>
                        <div className={'trick-winners'} onMouseOver={this.showGroupedTricks} onMouseOut={this.hideGroupedTricks}>
                            {
                                this.state.trickWinners.map((playerName: string, i: number) =>
                                    <div key={i}><b>Trick {i+1}</b> {playerName}</div>
                                )
                            }
                        </div>
                    </div>
                    <ActionPane gameId={this.state.gameId}
                        onHandEnd={this.handEnd}
                        onTrickEnd={this.trickEnd} />
                </div>
            </div>
        );
    }
}