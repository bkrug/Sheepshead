import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import Card from './Card';
import { PlayState, PickChoice, CardSummary } from './PlayState';

export interface PickPaneState {
    gameId: string;
    playerId: string;
    playerCards: CardSummary[];
    requestingPlayerTurn: boolean;
    buryCards: CardSummary[];
    partnerCard: CardSummary | null;
}

export interface PickPaneProps extends React.Props<any> {
    gameId: string;
    onBury: () => void;
}

export default class PickPane extends React.Component<PickPaneProps, PickPaneState> {
    private cardContainerStyle = { height: "96px" };

    constructor(props: PickPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            playerCards: [],
            requestingPlayerTurn: false,
            buryCards: [],
            partnerCard: null
        };
        this.buryChoice = this.buryChoice.bind(this);
        this.recordBuryChoice = this.recordBuryChoice.bind(this);
        this.initializePlayStatePinging = this.initializePlayStatePinging.bind(this);
        this.initializePlayStatePinging();
    }

    private initializePlayStatePinging(): void {
        var self = this;
        FetchUtils.repeatGet(
            'Game/GetBuryState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.setState({
                    playerCards: json.playerCards,
                    requestingPlayerTurn: json.requestingPlayerTurn
                });
                if (json.turnType != "Bury")
                    self.props.onBury();
            },
            function (json: PlayState): boolean {
                return json.requestingPlayerTurn == false && json.turnType == "Bury";
            },
            1000);
    }

    private buryChoice(card: Card): void {
        var buryList = this.state.buryCards;
        buryList.push(card.props.cardSummary);

        var heldList = this.state.playerCards;
        var index = heldList.indexOf(card.props.cardSummary);
        heldList.splice(index, 1);

        this.setState({
            buryCards: buryList,
            playerCards: heldList
        });
    }

    private recordBuryChoice(goItAlone: boolean): void {
        var self = this;
        var url = 'Game/RecordBury?gameId=' + this.state.gameId
            + '&playerId=' + this.state.playerId
            + '&cards=' + this.state.buryCards[0].name
            + '&cards=' + this.state.buryCards[1].name
            + '&goItAlone=' + goItAlone;
        FetchUtils.post(url,
            function (json: number[]): void {
                self.props.onBury();
            }
        );
    }

    private renderHeldCards() {
        return (
            <div>
                <b>Held Cards</b>
                <div style={this.cardContainerStyle}>
                {
                    this.state.playerCards.map((card: CardSummary, i: number) =>
                        <Card key={i} cardSummary={card} onClick={this.buryChoice} />
                    )
                }
                </div>
            </div>
        )
    }

    private renderCardMarkedForBury() {
        return (
            <div>
                <b>Cards to Bury</b>
                <div style={this.cardContainerStyle}>
                    {
                        this.state.buryCards.map((card: CardSummary, i: number) =>
                            <Card key={i} cardSummary={card} />
                        )
                    }
                </div>
            </div>
        );
    }

    private renderPartnerOption() {
        return (
            <div style={{ display: (this.state.buryCards.length == 2 ? 'block' : 'none') }}>
                <button onClick={() => this.recordBuryChoice(false)}>Play with Partner</button>
                <button onClick={() => this.recordBuryChoice(true)}>Go It Alone</button>
            </div>
        )
    }

    public render() {
        return (
            <div>
                {this.state.requestingPlayerTurn
                    ? <div>
                        <h4>Pick cards to bury</h4>
                        {this.renderHeldCards()}
                        {this.renderCardMarkedForBury()}
                        {this.renderPartnerOption()}
                    </div>
                    : <div><h4>Bury Phase</h4>Waiting for Picker to bury cards.</div>
                }
            </div>
        );
    }
}