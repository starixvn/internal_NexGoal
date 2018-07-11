import React, { Component, Fragment } from 'react';
import { Button, FormGroup } from 'react-bootstrap';

class SubmitForm extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        if (this.props.isHost) {
            return (
                <form className="form-inline form-filter">
                    <FormGroup>
                        <Button type="button" bsStyle="primary" onClick={this.props.handleSubmit}>Submit</Button>
                    </FormGroup>
                </form>
            );
        }

        return (<Fragment />);
    }
}

export default SubmitForm;