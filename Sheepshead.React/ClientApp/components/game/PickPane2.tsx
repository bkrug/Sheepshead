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
    pickChoices: PickChoice[];
    displayedPickChoices: PickChoice[];
    playerCards: CardSummary[];
    requestingPlayerTurn: boolean;
    turnType: string;
    currentTurn: string;
}

export interface PickPaneProps extends React.Props<any> {
    gameId: string;
    onPick: () => void;
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
            currentTurn: ''
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
            function (json: PlayState): void {
                self.setState({
                    pickChoices: json.pickChoices,
                    requestingPlayerTurn: json.requestingPlayerTurn,
                    playerCards: json.playerCards,
                    turnType: json.turnType,
                    currentTurn: json.currentTurn
                });
            },
            function (json: PlayState): boolean {
                return json.requestingPlayerTurn == false && (json.turnType == "Pick" || json.turnType == "BeginDeck");
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
        var pickPhaseComplete = this.state.turnType == "Bury" || this.state.turnType == "PlayTrick";
        if (allChoicesDisplayed && pickPhaseComplete)
            this.finishPickPhase(2000);
    }

    private pickChoice(willPick: boolean): void {
        var self = this;
        FetchUtils.post(
            'Game/RecordPickChoice?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId + '&willPick=' + willPick,
            function (json: number[]): void {
                if (willPick)
                    self.finishPickPhase(0);
                else
                    self.initializePlayStatePinging();
            }
        );
    }

    private finishPickPhase(timeout: number): void {
        clearInterval(this.displayInterval);
        setTimeout(this.props.onPick, timeout);
    }

    public render() {
        var allChoicesDisplayed = this.state.displayedPickChoices.length >= this.state.pickChoices.length;
        var waitingForAnotherPlayer =
            this.state.displayedPickChoices.length == this.state.pickChoices.length
            && this.state.currentTurn
            && !this.state.requestingPlayerTurn;

        return (
            <div>
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
                            ? <div>
                                <b>Do you want to pick?</b>
                                <button onClick={() => this.pickChoice(true)}>Yes</button>
                                <button onClick={() => this.pickChoice(false)}>No</button>
                            </div>
                            : <div></div>
                    }
                </div>
                {
                    this.state.playerCards.map((card: CardSummary, i: number) =>
                        <Card key={i} cardSummary={card} />
                    )
                }
            </div>
        );
    }
}