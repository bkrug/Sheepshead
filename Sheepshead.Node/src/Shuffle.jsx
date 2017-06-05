import React from 'react';

class Stack extends React.Component {
    renderFullCardBack() {
        return this.props.count > 0 ? (<div className="card-back-full" />) : (<div className="card-empty" />);
    }

    renderPartialCardBack(i) {
        return this.props.count >= i ? (<div className="card-back-part" />) : (<div />);
    }

    render() {
        return (
            <div className="card-stack">
                <h3>{this.props.title}</h3>
                {this.renderFullCardBack()}
                {this.renderPartialCardBack(2)}
                {this.renderPartialCardBack(3)}
                {this.renderPartialCardBack(4)}
                {this.renderPartialCardBack(5)}
                {this.renderPartialCardBack(6)}
            </div>
        )
    }
}

export default class Suffle extends React.Component {
    constructor(props) {
        super(props);
        this.state = { count: -1 };
    }

    componentDidMount() {
        this.timerID = setInterval(
            () => this.tick(),
            100
        );
    }

    componentWillUnmount() {
        clearInterval(this.timerID);
    }

    tick() {
        var newCount = this.state.count + 1;
        if (newCount > 32)
            return;
        var playerCardsPerBlind = 2;
        var playerCount = 5;
        var groupSize = playerCount * playerCardsPerBlind + 1;
        var modulus = newCount % groupSize;
        var baseCards = parseInt(newCount / groupSize, 10) * playerCardsPerBlind + (modulus > playerCount ? 1 : 0);
        var remainder = (modulus <= playerCount) ? modulus : modulus - playerCount;
        var blind = parseInt(newCount / groupSize, 10);
        this.setState({
            count: newCount,
            player1: baseCards + (remainder > 0 ? 1 : 0),
            player2: baseCards + (remainder > 1 ? 1 : 0),
            player3: baseCards + (remainder > 2 ? 1 : 0),
            player4: baseCards + (remainder > 3 ? 1 : 0),
            player5: baseCards + (remainder > 4 ? 1 : 0),
            blind: blind
        })
    }

    render() {
        return (
            <div>
                <h2>Shuffling Cards</h2>
                <Stack title="Frank" count={this.state.player1} />
                <Stack title="Mellisa" count={this.state.player2} />
                <Stack title="Joel" count={this.state.player3} />
                <Stack title="Roberto" count={this.state.player4} />
                <Stack title="Vivek" count={this.state.player5} />
                <Stack title="blind" count={this.state.blind} />
            </div>
        );
    }
}