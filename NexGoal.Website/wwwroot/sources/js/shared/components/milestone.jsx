import React, { Component } from 'react';

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

    onClickNextPrev = () => {
        this.setState({
            currentIndex: this.state.currentIndex - 1
        }, () => {
            this.setState({
                nextDisabled: this.state.currentIndex === 0,
                prevDisabled: this.state.currentIndex === this.state.maxIndex
            });
            this.props.onSelectionChange(this.props.selections[this.state.currentIndex].value);
        });
    }

    render() {
        const selections = this.props.selections;
        const nextDisabled = this.state.nextDisabled ? 'disabled' : '';
        const prevDisabled = this.state.prevDisabled ? 'disabled' : '';
        return (
            <div className="milestone">
                <button style={{ height: '15px', width: '15px', color: 'black', zIndex: 1000 }}
                    className={`arrow-left ${prevDisabled}`}
                    disabled={this.state.prevDisabled}
                    onClick={this.onClickNextPrev}
                />
                {selections[this.state.currentIndex].name}
                <button style={{ height: '15px', width: '15px', color: 'black', zIndex: 1000 }}
                    className={`arrow-right ${nextDisabled}`}
                    disabled={this.state.nextDisabled}
                    onClick={this.onClickNextPrev}
                />
            </div>
        );
    }
}