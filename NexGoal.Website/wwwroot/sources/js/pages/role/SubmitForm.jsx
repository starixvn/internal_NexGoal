import React, { Component, Fragment } from 'react';

class SubmitForm extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        if (this.props.isHost) {
            return (
                <form className="form-inline form-filter">
                    <div className="form-group">
                        <button type="button" className="btn btn-primary" onClick={this.props.handleSubmit}>Submit</button>
                    </div>
                </form>
            );
        }

        return (<Fragment />);
    }
}

export default SubmitForm;