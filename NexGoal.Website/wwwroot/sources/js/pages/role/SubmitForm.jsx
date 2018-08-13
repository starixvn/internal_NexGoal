import React from 'react';
import { Button, FormGroup } from 'react-bootstrap';

const SubmitForm = props => (props.isHost &&
    <form className="form-inline form-filter">
        <FormGroup>
            <Button type="button" bsStyle="primary" onClick={props.onSubmit}>Submit</Button>
        </FormGroup>
    </form>
);

export default SubmitForm;