import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import Card from './Card';
import { PickState, PickChoice, CardSummary } from './PlayState';

export interface PickPaneState {
    gameId: string;
    playerId: string;
    pickChoices: PickChoice[];
    displayedPickChoices: PickChoice[];
    playerCards: CardSummary[];
    requestingPlayerTurn: boolean;
    turnType: string;
    currentTurn: string;
    mustRedeal: boolean;
}

export interface PickPaneProps extends React.Props<any> {
    gameId: string;
    onPick: (mustRedeal: boolean) => void;
}

export default class PickPane extends React.Component<PickPaneProps, PickPaneState> {
    displayInterval: number;

    constructor(props: PickPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            pickChoices: [],
            playerCards: [],
            requestingPlayerTurn: false,
            displayedPickChoices: [],
            turnType: '',
            currentTurn: '',
            mustRedeal: false
        };
        this.pickChoice = this.pickChoice.bind(this);
        this.initializePlayStatePinging = this.initializePlayStatePinging.bind(this);
        this.initializePlayStatePinging();
        this.displayOneMorePlay = this.displayOneMorePlay.bind(this);
        this.displayInterval = setInterval(this.displayOneMorePlay, 1000);
    }

    private initializePlayStatePinging(): void {
        var self = this;
        FetchUtils.repeatGet(
            'Game/GetPickState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PickState): void {
                self.setState({
                    pickChoices: json.pickChoices,
                    requestingPlayerTurn: json.requestingPlayerTurn,
                    playerCards: json.playerCards,
                    turnType: json.turnType,
                    currentTurn: json.currentTurn,
                    mustRedeal: json.mustRedeal
                });
            },
            function (json: PickState): boolean {
                var validTurnTypes = ["Pick", "BeginHand"];
                return json.requestingPlayerTurn == false && validTurnTypes.indexOf(json.turnType) > -1 && !json.mustRedeal;
            },
            1000);
    }

    private displayOneMorePlay(): void {
        var picksToDisplay = this.state.displayedPickChoices.length + 1;

        var picks = this.state.pickChoices.slice(0, picksToDisplay);
        this.setState({
            displayedPickChoices: picks
        });

        var allChoicesDisplayed = this.state.displayedPickChoices.length >= this.state.pickChoices.length;
        var pickPhaseComplete = this.state.turnType == "Bury" || this.state.turnType == "PlayTrick" || this.state.mustRedeal;
        if (allChoicesDisplayed && pickPhaseComplete)
            this.finishPickPhase(2000, this.state.mustRedeal);
    }

    private pickChoice(willPick: boolean): void {
        var self = this;
        FetchUtils.post(
            'Game/RecordPickChoice?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId + '&willPick=' + willPick,
            function (json: number[]): void {
                if (willPick)
                    self.finishPickPhase(0, false);
                else
                    self.initializePlayStatePinging();
            }
        );

        this.state.displayedPickChoices.push({
            item1: IdUtils.getPlayerName(this.state.gameId) + '',
            item2: willPick
        });
        this.setState({
            displayedPickChoices: this.state.displayedPickChoices
        });
    }

    private finishPickPhase(timeout: number, mustRedeal: boolean): void {
        clearInterval(this.displayInterval);
        var self = this;
        setTimeout(function () { self.props.onPick(mustRedeal); }, timeout);
    }

    public render() {
        var allChoicesDisplayed = this.state.displayedPickChoices.length >= this.state.pickChoices.length;
        var waitingForAnotherPlayer =
            this.state.displayedPickChoices.length == this.state.pickChoices.length
            && this.state.currentTurn
            && !this.state.requestingPlayerTurn;

        return (
            <div className="pick-pane">
                <h4>Pick Phase</h4>
                {
                    Object.keys(this.state.displayedPickChoices).map((playerName, i) => (
                            <div key={i}>
                            <p>{this.state.displayedPickChoices[i].item1 + (this.state.displayedPickChoices[i].item2 ? ' picked.' : ' refused.')}</p>
                            </div>
                    ))
                }
                <div>
                {
                    waitingForAnotherPlayer                
                        ? this.state.currentTurn + ' is deciding whether to pick.'
                        : ''
                }
                </div>
                <div>
                    {
                        this.state.requestingPlayerTurn && allChoicesDisplayed
                            ? <div className="pick-option">
                                <b>Do you want to pick?</b>
                                <button onClick={() => this.pickChoice(true)}>Yes</button>
                                <button onClick={() => this.pickChoice(false)}>No</button>
                            </div>
                            : <div></div>
                    }
                </div>
                <div className="player-cards">
                    {
                        this.state.playerCards.map((card: CardSummary, i: number) =>
                            <Card key={i} cardSummary={card} />
                        )
                    }
                </div>
            </div>
        );
    }
}