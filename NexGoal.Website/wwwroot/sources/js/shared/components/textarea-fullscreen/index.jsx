import React, { Component } from 'react';

export default class TextareaFullscreenEditor extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isFullscreen: false
        };

        this.textareaEditor = React.createRef();
    }

    render() {
        return (
            <div className={`textarea-fullscreen-wrapper`}>
                <div className={`textarea-fullscreen-editor ${this.state.isFullscreen ? 'expanded' : ''}`}>
                    <textarea name={this.props.name} value={this.props.value} />
                </div >
            </div >
        );
    }
}