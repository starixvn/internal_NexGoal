import React, { Component, Fragment } from 'react';

export default class Milestone extends Component {
    constructor(props) {
        super(props);
        this.state = {
            currentIndex: 0,
            maxIndex: props.selections.length - 1,
            nextDisabled: true,
            prevDisabled: false
        };
    }

    onClickNext = () => {
        this.setState({
            currentIndex: this.state.currentIndex - 1,
            nextDisabled: this.state.currentIndex === 1
        }, () => this.props.onClickNext()
        );
    }

    onClickPrev = () => {
        this.setState({
            currentIndex: this.state.currentIndex + 1,
            nextDisabled: this.state.currentIndex === this.state.maxIndex
        }, () => this.props.onClickPrevious()
        );
    }

    render() {
        const selections = this.props.selections;
        const nextDisabled = this.state.nextDisabled ? 'disabled' : '';
        const prevDisabled = this.state.nextDisabled ? 'disabled' : '';
        return (
            <div className="milestone">
                <button className={`previous ${prevDisabled}`} onClick={this.onClickNext} />
                {selections[this.state.currentIndex].name}
                <button className={`next ${nextDisabled}`} onClick={this.onClickPrev} />
            </div>
        );
    }
}