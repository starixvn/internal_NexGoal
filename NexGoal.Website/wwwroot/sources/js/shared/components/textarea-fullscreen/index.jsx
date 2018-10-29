import React, { Component } from 'react';

export default class TextareaFullscreenEditor extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isFullscreen: false
        };

        this.$body = $(document.body);
        this.textareaEditor = React.createRef();
    }

    handleChangeMode = () => {
        const isFullscreen = this.state.isFullscreen;

        if (!isFullscreen) {
            this.$body.css({ 'overflow': 'hidden' });
            this.textareaEditor.current.focus();
        } else {
            this.$body.css({ 'overflow': '' });
        }

        this.setState(prevState => {
            return { isFullscreen: !prevState.isFullscreen };
        });
    }

    handleOnChange = (event) => {
        this.props.onChange(event);
    }

    render() {
        const isFullscreen = this.state.isFullscreen;

        return (
            <div className={`textarea-fullscreen-wrapper`}>
                {/* <button type="button" onClick={this.handleChangeMode}>{isFullscreen ? 'Minimize' : 'Maximize'}</button> */}
                <div className={`textarea-fullscreen-editor ${isFullscreen ? 'expanded' : ''}`}>
                    <div className="fullscreen-content">
                        <div className="header"></div>
                        <textarea ref={this.textareaEditor} className={this.props.className}
                            name={this.props.name}
                            value={this.props.value}
                            onChange={this.handleOnChange}
                        />
                    </div>
                    
                </div>
                <span className="icon-fullmode" onClick={this.handleChangeMode}></span>
            </div>
        );
    }
}