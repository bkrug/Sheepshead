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
    tricks: { key: string, value: CardSummary }[][];
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
                    leastersHand: json.leastersHand,
                    tricks: json.tricks
                });
            });
    }

    private showGroupedTricks(): void {
        if (this.state.tricks.length > 0 && this.state.tricks[0].length > 0)
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
        var playerList = this.state.tricks.map((trick: { key: string, value: CardSummary }[], i: number) =>
            <div key={i} className='trick-summary'>
                <div className='trick-number'><div className='trick-number'>{i+1})</div></div>
                {trick.map((moveSummary: { key: string, value: CardSummary }, j: number) =>
                    <div key={j} className={this.getCssClass(moveSummary.value) + ' move'}>
                        <div className='move-player'>{moveSummary.key}</div>
                        <div className='move-card'>{moveSummary.value.abbreviation.replace('T', '10')}</div>
                    </div>
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

    private renderPickerPartnerData() {
        if (this.state.leastersHand)
            return (<div className={'hand-data'}><b>Leasters Hand</b></div>);
        if (this.state.partnerCard)
            return (<div className={'hand-data'}>
                <div>Picker: {this.state.picker}</div>
                <div>Partner: {this.state.partner}</div>
                <div>Partner Card: {this.state.partnerCard}</div>
            </div>);
        return (<div className={'hand-data'}>
            <div>Picker: {this.state.picker}</div>
        </div>);
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
                                <div key={i} className='playerCoins'>
                                    {coinScore.name}
                                    <br />
                                    {coinScore.score}
                                </div>
                            )
                        }
                    </div>
                    { this.state.showGroupedTricks ? this.renderModal() : <div></div> }
                    <div>
                        <h4>Hand Details</h4>
                        { this.renderPickerPartnerData() }
                        <div className={'trick-winners'}>
                            <div onMouseOver={this.showGroupedTricks} onMouseOut={this.hideGroupedTricks}>
                            {
                                this.state.trickWinners.map((playerName: string, i: number) =>
                                    <div key={i}><a>Trick {i+1}</a> {playerName}</div>
                                )
                            }
                            </div>
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