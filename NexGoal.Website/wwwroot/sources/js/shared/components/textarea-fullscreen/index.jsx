import React, { Component } from 'react';
import PropTypes from 'prop-types';

export default class TextareaFullscreenEditor extends Component {
    static propTypes = {
        className: PropTypes.string,
        name: PropTypes.string,
        value: PropTypes.string,
        onChange: PropTypes.func
    };

    constructor(props) {
        super(props);

        this.state = {
            isFullscreen: false
        };

        this.$body = $(document.body);
        this.textareaEditor = React.createRef();
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (this.props.className !== nextProps.className
            || this.props.name !== nextProps.name
            || this.props.value !== nextProps.value
            || this.state.isFullscreen !== nextState.isFullscreen) {
            return true;
        }

        return false;
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
            <div className="textarea-fullscreen-wrapper">
                <div className={`textarea-fullscreen-editor ${isFullscreen ? 'expanded' : ''}`}>
                    <div className="fullscreen-content">
                        <div className="header" onClick={this.handleChangeMode} title={isFullscreen ? 'Minimize' : 'Maximize'} />
                        <textarea ref={this.textareaEditor}
                            rows="5"
                            className={this.props.className}
                            name={this.props.name}
                            value={this.props.value}
                            disabled={this.props.disabled}
                            onChange={this.handleOnChange}
                        />
                    </div>

                </div>
                < span className="icon-fullmode" onClick={this.handleChangeMode} title={isFullscreen ? 'Minimize' : 'Maximize'} />
            </div>
        );
    }
}